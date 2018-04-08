using System;
using System.Collections.Generic;

namespace Bridge.DataLayer.Models
{
    public partial class Events
    {
        public long InStoreId { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public Guid ProcessId { get; set; }
        public string TypeName { get; set; }
        public string EventDescription { get; set; }
        public DateTime OccurredOn { get; set; }
        public string EventBody { get; set; }
    }
}
