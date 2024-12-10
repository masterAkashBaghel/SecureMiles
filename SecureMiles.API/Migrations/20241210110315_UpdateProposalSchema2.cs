using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureMiles.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProposalSchema2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PremiumAmount",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PremiumAmount",
                table: "Proposals");
        }
    }
}
