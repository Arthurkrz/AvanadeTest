using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.API.Architecture.Migrations
{
    /// <inheritdoc />
    public partial class CreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CPF = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(256)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(128)", nullable: false),
                    HashAlgorithm = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HashParams = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FailedLoginCount = table.Column<int>(type: "int", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_CPF",
                table: "Administrators",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_Name",
                table: "Administrators",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_Username",
                table: "Administrators",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrators");
        }
    }
}
