using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreDataToOrderStatusEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "StatusId", "StatusName" },
                values: new object[,]
                {
                    { 4, "Cancelled" },
                    { 5, "Confirm" },
                    { 6, "Delivered" }
                });

           /* migrationBuilder.UpdateData(
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
                value: "$2a$11$qDQ7PM2NRdIpH/pRenCHy.UCcpfTmoZR8ho1LrbVxP9CyGKlRbAuq");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "StatusId",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"),
                column: "PasswordHash",
                value: "$2a$11$DUEtfo/4/wNxyTV8m9LPaOwBZE2tD70d5isPom.7zNidXyhINp.i2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"),
                column: "PasswordHash",
                value: "$2a$11$kgQPkJfrAw91JMh1uyMtUObEXmYk8vRptVWzV334WDQ4udj/LuEgm");
        }
    }
}
