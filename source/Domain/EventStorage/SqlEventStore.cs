#region (c) 2012-2012 Lokad - New BSD License 

// Copyright (c) Lokad 2012-2012, http://www.lokad.com
// This code is released as Open Source under the terms of the New BSD Licence

#endregion

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Domain.Contracts;

namespace Domain.EventStorage
{
    /// <summary>
    /// <para>This is a SQL event storage simplified to demonstrate essential principles.
    /// If you need a more robust MS SQL implementation, check out Event Store of
    /// Jonathan Oliver</para>
    /// <para>This code is frozen to match IDDD book. For latest practices see Lokad.CQRS Project</para>
    /// </summary>
    /// <summary>
    /// <para>This is a SQL event storage simplified to demonstrate essential principles.
    /// If you need a more robust MS SQL implementation, check out Event Store of
    /// Jonathan Oliver</para>
    /// <para>This code is frozen to match IDDD book. For latest practices see Lokad.CQRS Project</para>
    /// </summary>
    public sealed class SqlAppendOnlyStore : IAppendOnlyStore
    {
        readonly string _connectionString;


        public SqlAppendOnlyStore(string connectionString, bool init = false)
        {
            _connectionString = connectionString;

            if (init) Initialize();

        }

        public void Initialize()
        {

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                const string txt =
                    @"IF NOT EXISTS 
                        (SELECT * FROM sys.objects 
                            WHERE object_id = OBJECT_ID(N'[dbo].[Events]') 
                            AND type in (N'U'))

                        CREATE TABLE [dbo].[Events](
                            [InStoreId] [bigint] PRIMARY KEY IDENTITY,
                            [EventId] [int] NOT NULL,
                            [UserId] [int] NOT NULL,
                            [ProcessId] [uniqueidentifier] NOT NULL,
	                        [TypeName] [nvarchar](200) NOT NULL,
	                        [EventDescription] [nvarchar](500) NOT NULL,
	                        [OccurredOn] [DateTime] NOT NULL,
	                        [EventBody] [nvarchar](max) NOT NULL
                        ) ON [PRIMARY]
";
                using (var cmd = new SqlCommand(txt, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<StoredEvent> ReadRecords(Guid fromSessionId, int maxCount = 1)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                const string sql =
                    @"SELECT TOP (@take) TypeName,OccurredOn,EventDescription,EventId, EventBody, InStoreId,ProcessId, UserId FROM Events with(nolock)
                        WHERE [ProcessId] = @fromSessionId
                        ORDER BY InStoreId DESC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@take", maxCount);
                    cmd.Parameters.AddWithValue("@fromSessionId", fromSessionId);


                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var eventDescription = (string)reader["EventDescription"];
                            var eventBody = (string)reader["EventBody"];
                            var typeName = (string)reader["TypeName"];
                            var eventId = (int)reader["EventId"];
                            var occurredOn = (DateTime)reader["OccurredOn"];
                            var inStoreId = (long)reader["InStoreId"];
                            var sessionId = (Guid)reader["ProcessId"];
                            var userId = (int)reader["UserId"];

                            yield return new StoredEvent(typeName, occurredOn, eventDescription, eventId, eventBody,userId, sessionId, inStoreId);
                        }
                    }
                }
            }
        }

        public IEnumerable<StoredEvent> ReadRecords(int forEventId, int maxCount = 1)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                const string sql =
                    @"SELECT TOP (@take) TypeName,OccurredOn,EventDescription,EventId, EventBody, InStoreId,ProcessId, UserId FROM Events with(nolock)
                        WHERE EventId = @fromEventId
                        ORDER BY InStoreId DESC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@take", maxCount);
                    cmd.Parameters.AddWithValue("@fromEventId", forEventId);


                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var eventDescription = (string)reader["EventDescription"];
                            var eventBody = (string)reader["EventBody"];
                            var typeName = (string)reader["TypeName"];
                            var eventId = (int)reader["EventId"];
                            var occurredOn = (DateTime)reader["OccurredOn"];
                            var inStoreId = (long)reader["InStoreId"];
                            var processId = (Guid)reader["ProcessId"];
                            var userId = (int)reader["UserId"];

                            yield return new StoredEvent(typeName, occurredOn, eventDescription, eventId, eventBody,userId, processId, inStoreId);
                        }
                    }
                }
            }
        }


        public void Append(StoredEvent storedEvent)
        {
            var eventBody = storedEvent.EventBody;
            var typeName = storedEvent.TypeName;
            var eventId = storedEvent.EventId;
            var sessionId = storedEvent.ProcessId;
            var occurredOn = storedEvent.OccurredOn;
            var userId = storedEvent.UserId;
            var eventDescription = storedEvent.EventDescription;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    const string txt =
                           @"INSERT INTO Events (TypeName,OccurredOn,EventDescription,EventId, EventBody, ProcessId, UserId) 
                                VALUES(@TypeName,@OccurredOn,@EventDescription,@EventId,@EventBody, @ProcessId, @UserId)";

                    using (var cmd = new SqlCommand(txt, conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@EventBody", eventBody);
                        cmd.Parameters.AddWithValue("@TypeName", typeName);
                        cmd.Parameters.AddWithValue("@EventId", eventId);
                        cmd.Parameters.AddWithValue("@OccurredOn", occurredOn);
                        cmd.Parameters.AddWithValue("@EventDescription", eventDescription);
                        cmd.Parameters.AddWithValue("@ProcessId", sessionId);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
            }
        }

        public long Count(int? eventId = null)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                const string sql =
                    @"SELECT Count(InStoreId) AS EventCount FROM Events
                        WHERE EventId = @fromEventId
";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@fromEventId", eventId != null ? (object)eventId.Value : DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return (long)reader["EventCount"];
                        }
                    }
                }
            }

            return 0;
        }

        public bool Close()
        {
            return false;
        }
    }
}