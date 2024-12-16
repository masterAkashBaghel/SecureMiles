using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureMiles.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProposalSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PolicyType",
                table: "Proposals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PolicyType",
                table: "Proposals");
        }
    }
}
