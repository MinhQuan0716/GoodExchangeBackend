using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifySubscriptionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subcriptions_WalletTransactions_WalletTransactionId",
                table: "Subcriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subcriptions_WalletTransactionId",
                table: "Subcriptions");

            migrationBuilder.DropColumn(
                name: "WalletTransactionId",
                table: "Subcriptions");

            migrationBuilder.AddColumn<Guid>(
                name: "SubscriptionId",
                table: "WalletTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_SubscriptionId",
                table: "WalletTransactions",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Subcriptions_SubscriptionId",
                table: "WalletTransactions",
                column: "SubscriptionId",
                principalTable: "Subcriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Subcriptions_SubscriptionId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_SubscriptionId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "WalletTransactions");

            migrationBuilder.AddColumn<Guid>(
                name: "WalletTransactionId",
                table: "Subcriptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subcriptions_WalletTransactionId",
                table: "Subcriptions",
                column: "WalletTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subcriptions_WalletTransactions_WalletTransactionId",
                table: "Subcriptions",
                column: "WalletTransactionId",
                principalTable: "WalletTransactions",
                principalColumn: "Id");
        }
    }
}
