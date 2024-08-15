using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryMonth",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "SubscriptionHistories");

            migrationBuilder.AddColumn<int>(
                name: "ExpiryDay",
                table: "Subscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsExtend",
                table: "SubscriptionHistories",
                type: "bit",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"),
                column: "PasswordHash",
                value: "$2a$11$3LJuN68dT.R80iXt.kKL2uvfkOQPhVwWpPdb1faAyWDtcyD.vuUEq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"),
                column: "PasswordHash",
                value: "$2a$11$93y9G0m.76OY3LOi22pOF./NO5ia6Cnhgs48wZ0K9Ze7yXCGD/QwC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDay",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "IsExtend",
                table: "SubscriptionHistories");

            migrationBuilder.AddColumn<float>(
                name: "ExpiryMonth",
                table: "Subscriptions",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<long>(
                name: "Rate",
                table: "SubscriptionHistories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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
    }
}
