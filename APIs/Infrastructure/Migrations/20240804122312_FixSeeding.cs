using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8438eac1-0ff7-455d-ae6b-32cae394373e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ab65dd21-a7ac-4f46-bf84-cf36674af0e6"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDay", "CreatedBy", "CreationDate", "DeletedBy", "DeletetionDate", "Email", "FirstName", "HomeAddress", "IsBuisnessAccount", "IsDelete", "LastName", "ModificationBy", "ModificationDate", "PasswordHash", "PhoneNumber", "ProfileImage", "RoleId", "UserName", "VerifyUserId", "WalletId" },
                values: new object[,]
                {
                    { new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"), null, null, null, null, null, "admin@gmail.com", null, null, null, false, null, null, null, "$2a$11$hanjdmGElOlMm777B0nPje3BaknjC1iB0Vt2Sq5hl7YLifjZ8B1nG", null, null, 1, "Admin", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"), null, null, null, null, null, "moderator@gmail.com", null, null, null, false, null, null, null, "$2a$11$szP08ouBbXAX6s5KkM98Feef599AJDYfP3VTf/2IRERFiLUUIgJEG", null, null, 2, "Moderator", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("27aa7437-5d36-4a01-80e8-7f3e572f6d5c"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("319d5597-f149-4fa5-9c05-60e4f7120b8f"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDay", "CreatedBy", "CreationDate", "DeletedBy", "DeletetionDate", "Email", "FirstName", "HomeAddress", "IsBuisnessAccount", "IsDelete", "LastName", "ModificationBy", "ModificationDate", "PasswordHash", "PhoneNumber", "ProfileImage", "RoleId", "UserName", "VerifyUserId", "WalletId" },
                values: new object[,]
                {
                    { new Guid("8438eac1-0ff7-455d-ae6b-32cae394373e"), null, null, null, null, null, "moderator@gmail.com", null, null, null, false, null, null, null, "$2a$11$jnq.MX2Tm.g16PTotPZTqONvf1bVyj3jfEHeWh0SqiF0oItIspheS", null, null, 2, "Moderator", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("ab65dd21-a7ac-4f46-bf84-cf36674af0e6"), null, null, null, null, null, "admin@gmail.com", null, null, null, false, null, null, null, "$2a$11$mMu9lJN3bTUsShtbQahUxeN0ZvYcGGUTPPOZrmj.Bp0b7cUedChq.", null, null, 1, "Admin", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }
    }
}
