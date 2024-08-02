using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDataToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "WalletId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDay", "CreatedBy", "CreationDate", "DeletedBy", "DeletetionDate", "Email", "FirstName", "HomeAddress", "IsBuisnessAccount", "IsDelete", "LastName", "ModificationBy", "ModificationDate", "PasswordHash", "PhoneNumber", "ProfileImage", "RoleId", "UserName", "VerifyUserId", "WalletId" },
                values: new object[,]
                {
                    { new Guid("b5592252-ebc5-4003-99a1-a73a6d5fb6c1"), null, null, null, null, null, "admin@gmail.com", null, null, null, null, null, null, null, "$2a$11$A.yKimO/daJEju7OUHzq2OZ/vuL/EmT8C0kkJUIUqF6DO.mgtblom", null, null, 1, "Admin", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("c287ac57-0853-40b9-b91f-007e7af95f84"), null, null, null, null, null, "moderator@gmail.com", null, null, null, null, null, null, null, "$2a$11$aL313Pyul7dOHO3jvy37ze6c1B5AnIlCNyuY34BKF.dIK1R15ZPWK", null, null, 2, "Moderator", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b5592252-ebc5-4003-99a1-a73a6d5fb6c1"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c287ac57-0853-40b9-b91f-007e7af95f84"));

            migrationBuilder.AlterColumn<Guid>(
                name: "WalletId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
