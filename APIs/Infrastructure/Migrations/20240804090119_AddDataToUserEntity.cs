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
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3128a6d8-276c-42a5-8d5c-9418b34125b3"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("67b737ab-61a3-4661-adda-23ce7e1f010d"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDay", "CreatedBy", "CreationDate", "DeletedBy", "DeletetionDate", "Email", "FirstName", "HomeAddress", "IsBuisnessAccount", "IsDelete", "LastName", "ModificationBy", "ModificationDate", "PasswordHash", "PhoneNumber", "ProfileImage", "RoleId", "UserName", "VerifyUserId", "WalletId" },
                values: new object[,]
                {
                    { new Guid("3d467fb4-a342-4fe1-b05b-563349065dd4"), null, null, null, null, null, "admin@gmail.com", null, null, null, false, null, null, null, "$2a$11$soYs4e4aFsfmbHXX613q/u1oTQYlxHUda3G0M0RZBrH0wVS9n/W36", null, null, 1, "Admin", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("8638a4cb-117a-4e7b-b8e6-157188db8f63"), null, null, null, null, null, "moderator@gmail.com", null, null, null, false, null, null, null, "$2a$11$I0ul7peKTWUXv5FUcvtCeOAP/28OOlrMSm4V7mC0Obj/1/l/oZLf2", null, null, 2, "Moderator", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3d467fb4-a342-4fe1-b05b-563349065dd4"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8638a4cb-117a-4e7b-b8e6-157188db8f63"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDay", "CreatedBy", "CreationDate", "DeletedBy", "DeletetionDate", "Email", "FirstName", "HomeAddress", "IsBuisnessAccount", "IsDelete", "LastName", "ModificationBy", "ModificationDate", "PasswordHash", "PhoneNumber", "ProfileImage", "RoleId", "UserName", "VerifyUserId", "WalletId" },
                values: new object[,]
                {
                    { new Guid("3128a6d8-276c-42a5-8d5c-9418b34125b3"), null, null, null, null, null, "moderator@gmail.com", null, null, null, false, null, null, null, "$2a$11$SAnPrD4JuWvFRKJKPECciesUY2SexVuX24YY7FHaHZzAH/c6lVhoS", null, null, 2, "Moderator", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("67b737ab-61a3-4661-adda-23ce7e1f010d"), null, null, null, null, null, "admin@gmail.com", null, null, null, false, null, null, null, "$2a$11$i58QvavtX1bAM1.3IoTDae3a0400yBz7YC45DlTQY5x5/u1VyGN22", null, null, 1, "Admin", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }
    }
}
