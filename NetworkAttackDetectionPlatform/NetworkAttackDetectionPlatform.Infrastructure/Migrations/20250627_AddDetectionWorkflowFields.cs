using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace NetworkAttackDetectionPlatform.Infrastructure.Migrations
{
    [DbContext(typeof(NetworkAttackDetectionPlatform.Infrastructure.Data.ApplicationDbContext))]
    [Migration("20250627_AddDetectionWorkflowFields")]
    public partial class AddDetectionWorkflowFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AttackDetections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AttackDetections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AttackDetections",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AttackDetections");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AttackDetections");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AttackDetections");
        }
    }
}
