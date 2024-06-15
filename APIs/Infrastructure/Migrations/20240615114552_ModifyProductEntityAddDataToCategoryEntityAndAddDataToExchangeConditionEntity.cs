using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyProductEntityAddDataToCategoryEntityAndAddDataToExchangeConditionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName" },
                values: new object[,]
                {
                    { 5, "Home and Kitchen" },
                    { 6, "Toys and Games" },
                    { 7, "Books" }
                });

            migrationBuilder.UpdateData(
                table: "ExchangeConditions",
                keyColumn: "ConditionId",
                keyValue: 1,
                column: "ConditionType",
                value: "For selling");

            migrationBuilder.UpdateData(
                table: "ExchangeConditions",
                keyColumn: "ConditionId",
                keyValue: 2,
                column: "ConditionType",
                value: "For exchanging");

            migrationBuilder.InsertData(
                table: "ExchangeConditions",
                columns: new[] { "ConditionId", "ConditionType" },
                values: new object[] { 3, "For donation" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ExchangeConditions",
                keyColumn: "ConditionId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "ProductDescription",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "ExchangeConditions",
                keyColumn: "ConditionId",
                keyValue: 1,
                column: "ConditionType",
                value: "For exchanging");

            migrationBuilder.UpdateData(
                table: "ExchangeConditions",
                keyColumn: "ConditionId",
                keyValue: 2,
                column: "ConditionType",
                value: "For donation");
        }
    }
}
