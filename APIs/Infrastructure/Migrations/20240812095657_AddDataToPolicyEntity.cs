using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDataToPolicyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Policies",
                columns: new[] { "Id", "CreatedBy", "CreationDate", "DeletedBy", "DeletetionDate", "IsDelete", "ModificationBy", "ModificationDate", "OrderCancelledAmount", "PostPrice" },
                values: new object[] { new Guid("7644306e-6bd0-4438-a197-9d14a6a564fd"), null, null, null, null, false, null, null, 1, 15000f });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"),
                column: "PasswordHash",
                value: "$2a$11$li6qaZVvZoLoHktEeny6Q.xEFvix45xIi.N3MLAWa2SUFcxQ1I/V2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"),
                column: "PasswordHash",
                value: "$2a$11$kRhL4S/kG5rnNPUn3Oo2Z.qsYPcxtrVOySvqlPZMhsDmn0M4AKwJG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Policies",
                keyColumn: "Id",
                keyValue: new Guid("7644306e-6bd0-4438-a197-9d14a6a564fd"));

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
    }
}
