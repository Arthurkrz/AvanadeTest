using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sales.API.Architecture.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SaleCode = table.Column<int>(type: "int", nullable: false),
                    BuyerCPF = table.Column<int>(type: "int", nullable: false),
                    ProductCode = table.Column<int>(type: "int", nullable: false),
                    SellAmount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_ID",
                table: "Sales",
                column: "ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SaleCode",
                table: "Sales",
                column: "SaleCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sales");
        }
    }
}
