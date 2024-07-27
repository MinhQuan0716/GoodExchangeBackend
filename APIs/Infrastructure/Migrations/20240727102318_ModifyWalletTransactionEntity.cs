using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyWalletTransactionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
             name: "FK_SubcriptionHistories_Subcriptions_SubcriptionId",
             table: "SubcriptionHistories");
            migrationBuilder.DropForeignKey(
               name: "FK_WalletTransactions_Subcriptions_SubscriptionId",
               table: "WalletTransactions");
            migrationBuilder.DropTable(
                name: "Subcriptions");
            migrationBuilder.AddColumn<float>(
                name: "Amount",
                table: "WalletTransactions",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "WalletTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubcriptionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryMonth = table.Column<float>(type: "real", nullable: false),
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
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_OrderId",
                table: "WalletTransactions",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubcriptionHistories_Subscriptions_SubcriptionId",
                table: "SubcriptionHistories",
                column: "SubcriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Orders_OrderId",
                table: "WalletTransactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Subscriptions_SubscriptionId",
                table: "WalletTransactions",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubcriptionHistories_Subscriptions_SubcriptionId",
                table: "SubcriptionHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Orders_OrderId",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Subscriptions_SubscriptionId",
                table: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_OrderId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "WalletTransactions");

            migrationBuilder.CreateTable(
                name: "Subcriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletetionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryMonth = table.Column<float>(type: "real", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: true),
                    ModificationBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    SubcriptionType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcriptions", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_SubcriptionHistories_Subcriptions_SubcriptionId",
                table: "SubcriptionHistories",
                column: "SubcriptionId",
                principalTable: "Subcriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Subcriptions_SubscriptionId",
                table: "WalletTransactions",
                column: "SubscriptionId",
                principalTable: "Subcriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
