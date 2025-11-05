using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.API.Architecture.Migrations
{
    /// <inheritdoc />
    public partial class CreateUsernameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Buyers_Username",
                table: "Buyers",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Buyers_Username",
                table: "Buyers");
        }
    }
}
