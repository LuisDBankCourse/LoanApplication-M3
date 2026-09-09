using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoanApplications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ErrorMaxLength7000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "OutBoxMessages",
                type: "character varying(7000)",
                maxLength: 7000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "OutBoxMessages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(7000)",
                oldMaxLength: 7000,
                oldNullable: true);
        }
    }
}
