using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureMiles.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Policies_PolicyID",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "TransactionID",
                table: "Payments",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "PolicyID",
                table: "Payments",
                newName: "PolicyId");

            migrationBuilder.RenameColumn(
                name: "PaymentID",
                table: "Payments",
                newName: "PaymentId");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "Payments",
                newName: "Currency");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PolicyID",
                table: "Payments",
                newName: "IX_Payments_PolicyId");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Policies_PolicyId",
                table: "Payments",
                column: "PolicyId",
                principalTable: "Policies",
                principalColumn: "PolicyID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Policies_PolicyId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Payments",
                newName: "TransactionID");

            migrationBuilder.RenameColumn(
                name: "PolicyId",
                table: "Payments",
                newName: "PolicyID");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Payments",
                newName: "PaymentID");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "Payments",
                newName: "PaymentMethod");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PolicyId",
                table: "Payments",
                newName: "IX_Payments_PolicyID");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionID",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Policies_PolicyID",
                table: "Payments",
                column: "PolicyID",
                principalTable: "Policies",
                principalColumn: "PolicyID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
