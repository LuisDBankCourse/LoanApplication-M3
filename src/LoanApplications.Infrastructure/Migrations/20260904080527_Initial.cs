using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanApplications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreditProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TermMonths = table.Column<int>(type: "integer", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    RiskLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DecisionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EvaluatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutBoxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Error = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutBoxMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_Loan_CorrelationId",
                table: "LoanApplications",
                column: "CorrelationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Loan_IdempotencyKey",
                table: "LoanApplications",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutBox_ProcessedAtUtc",
                table: "OutBoxMessages",
                column: "ProcessedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanApplications");

            migrationBuilder.DropTable(
                name: "OutBoxMessages");
        }
    }
}
