using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostPrice = table.Column<float>(type: "real", nullable: true),
                    OrderCancelledAmount = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletetionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModificationBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"),
                column: "PasswordHash",
                value: "$2a$11$NbxqFo15BM3H0TOjeu7Z5ujDgPkiEF44Z41b9gKYkj5lvVL8zJhoW");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"),
                column: "PasswordHash",
                value: "$2a$11$OFHMG08rMvMWQXGLP61.qOmWfU3vO7.ONkEoCdMXCTho5DXhWRpBi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Posts");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"),
                column: "PasswordHash",
                value: "$2a$11$3tN8xru7Un1e5ze5PkEd1O0FESS09HZFkpeLJ3QBIsAdhPZTZ2hUa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"),
                column: "PasswordHash",
                value: "$2a$11$qDQ7PM2NRdIpH/pRenCHy.UCcpfTmoZR8ho1LrbVxP9CyGKlRbAuq");
        }
    }
}
