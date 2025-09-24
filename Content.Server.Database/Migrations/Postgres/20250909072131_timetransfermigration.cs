using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
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
                    server_public_key = table.Column<string>(type: "text", nullable: false),
                    server_name = table.Column<string>(type: "text", nullable: false),
                    enabled = table.Column<bool>(type: "boolean", nullable: false),
                    auto_approve_transfers = table.Column<bool>(type: "boolean", nullable: false),
                    application_max_age = table.Column<TimeSpan>(type: "interval", nullable: false),
                    role_transfer_data = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_transfer_server_info", x => x.server_public_key);
                });

            migrationBuilder.CreateTable(
                name: "time_transfers",
                columns: table => new
                {
                    time_transfers_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    raw = table.Column<string>(type: "text", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version = table.Column<string>(type: "text", nullable: false),
                    signature = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    approved = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "text", nullable: false)
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
