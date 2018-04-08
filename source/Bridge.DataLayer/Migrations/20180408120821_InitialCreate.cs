using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bridge.DataLayer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    InStoreId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventBody = table.Column<string>(nullable: false),
                    EventDescription = table.Column<string>(maxLength: 500, nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ProcessId = table.Column<Guid>(nullable: false),
                    TypeName = table.Column<string>(maxLength: 200, nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.InStoreId);
                });

            migrationBuilder.CreateTable(
                name: "SysEventType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    EventType = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysEventType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysPlayer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Player = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysPlayer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysVulnerability",
                columns: table => new
                {
                    SysVulnerabilityId = table.Column<int>(nullable: false),
                    SysVulnerabilityName = table.Column<string>(type: "nchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysVulnerability", x => x.SysVulnerabilityId);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    IsImported = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    SysEventTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_EventType",
                        column: x => x.SysEventTypeId,
                        principalTable: "SysEventType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BestContract = table.Column<string>(maxLength: 50, nullable: false),
                    BestContractDeclarer = table.Column<int>(nullable: false),
                    BestContractDisplay = table.Column<string>(maxLength: 50, nullable: false),
                    BestContractHandViewerInput = table.Column<string>(nullable: false),
                    BestContractResult = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false),
                    HandViewerInput = table.Column<string>(nullable: false),
                    Index = table.Column<int>(nullable: false),
                    PBNRepresentation = table.Column<string>(nullable: false),
                    SysVulnerabilityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deal_SysPlayer",
                        column: x => x.BestContractDeclarer,
                        principalTable: "SysPlayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deal_Event",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deal_SysVulnerability",
                        column: x => x.SysVulnerabilityId,
                        principalTable: "SysVulnerability",
                        principalColumn: "SysVulnerabilityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pair",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    Player1Name = table.Column<string>(maxLength: 500, nullable: false),
                    Player2Name = table.Column<string>(maxLength: 500, nullable: false),
                    Rank = table.Column<int>(nullable: false),
                    Score = table.Column<decimal>(type: "decimal(5, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pair", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pair_Event",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MakeableContract",
                columns: table => new
                {
                    MakeableContractId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Contract = table.Column<string>(maxLength: 10, nullable: false),
                    DealId = table.Column<int>(nullable: false),
                    Declarer = table.Column<int>(nullable: false),
                    Denomination = table.Column<int>(nullable: false),
                    HandViewerInput = table.Column<string>(nullable: false),
                    Level = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MakeableContract", x => x.MakeableContractId);
                    table.ForeignKey(
                        name: "FK_MakeableContract_Deal",
                        column: x => x.DealId,
                        principalTable: "Deal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MakeableContract_SysPlayer",
                        column: x => x.Declarer,
                        principalTable: "SysPlayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DuplicateDeal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Contract = table.Column<string>(maxLength: 50, nullable: false),
                    ContractDisplay = table.Column<string>(maxLength: 50, nullable: false),
                    DealId = table.Column<int>(nullable: false),
                    Declarer = table.Column<int>(nullable: false),
                    EWPairId = table.Column<int>(nullable: false),
                    EWPercentage = table.Column<int>(nullable: false),
                    HandViewerInput = table.Column<string>(nullable: false),
                    NSPairId = table.Column<int>(nullable: false),
                    NSPercentage = table.Column<int>(nullable: false),
                    Result = table.Column<int>(nullable: false),
                    Tricks = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DuplicateDeal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DuplicateDeal_Deal",
                        column: x => x.DealId,
                        principalTable: "Deal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DuplicateDeal_Declarer",
                        column: x => x.Declarer,
                        principalTable: "SysPlayer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DuplicateDeal_EWPair",
                        column: x => x.EWPairId,
                        principalTable: "Pair",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DuplicateDeal_NSPair",
                        column: x => x.NSPairId,
                        principalTable: "Pair",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deal_BestContractDeclarer",
                table: "Deal",
                column: "BestContractDeclarer");

            migrationBuilder.CreateIndex(
                name: "IX_Deal_EventId",
                table: "Deal",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Deal_SysVulnerabilityId",
                table: "Deal",
                column: "SysVulnerabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateDeal_DealId",
                table: "DuplicateDeal",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateDeal_Declarer",
                table: "DuplicateDeal",
                column: "Declarer");

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateDeal_EWPairId",
                table: "DuplicateDeal",
                column: "EWPairId");

            migrationBuilder.CreateIndex(
                name: "IX_DuplicateDeal_NSPairId",
                table: "DuplicateDeal",
                column: "NSPairId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_SysEventTypeId",
                table: "Event",
                column: "SysEventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MakeableContract_DealId",
                table: "MakeableContract",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_MakeableContract_Declarer",
                table: "MakeableContract",
                column: "Declarer");

            migrationBuilder.CreateIndex(
                name: "IX_Pair_EventId",
                table: "Pair",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuplicateDeal");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "MakeableContract");

            migrationBuilder.DropTable(
                name: "Pair");

            migrationBuilder.DropTable(
                name: "Deal");

            migrationBuilder.DropTable(
                name: "SysPlayer");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "SysVulnerability");

            migrationBuilder.DropTable(
                name: "SysEventType");
        }
    }
}
