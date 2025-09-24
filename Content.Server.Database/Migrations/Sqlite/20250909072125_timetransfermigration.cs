using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class timetransfermigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "time_transfer_server_info",
                columns: table => new
                {
                    server_public_key = table.Column<string>(type: "TEXT", nullable: false),
                    server_name = table.Column<string>(type: "TEXT", nullable: false),
                    enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    auto_approve_transfers = table.Column<bool>(type: "INTEGER", nullable: false),
                    application_max_age = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    role_transfer_data = table.Column<string>(type: "jsonb", nullable: false),
                    note = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_transfer_server_info", x => x.server_public_key);
                });

            migrationBuilder.CreateTable(
                name: "time_transfers",
                columns: table => new
                {
                    time_transfers_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    raw = table.Column<string>(type: "TEXT", nullable: false),
                    player_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    version = table.Column<string>(type: "TEXT", nullable: false),
                    signature = table.Column<string>(type: "TEXT", nullable: false),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    approved = table.Column<bool>(type: "INTEGER", nullable: false),
                    note = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_transfers", x => x.time_transfers_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "time_transfer_server_info");

            migrationBuilder.DropTable(
                name: "time_transfers");
        }
    }
}
