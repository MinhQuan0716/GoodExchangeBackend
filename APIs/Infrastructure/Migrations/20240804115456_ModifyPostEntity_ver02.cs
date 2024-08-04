using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyPostEntity_ver02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("46a64b11-6372-4d51-a995-4cc5f21290b6"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4f7f8695-f8b0-4a22-9b3d-95bf119f764d"));

            migrationBuilder.AlterColumn<bool>(
                name: "IsPriority",
                table: "Posts",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDay", "CreatedBy", "CreationDate", "DeletedBy", "DeletetionDate", "Email", "FirstName", "HomeAddress", "IsBuisnessAccount", "IsDelete", "LastName", "ModificationBy", "ModificationDate", "PasswordHash", "PhoneNumber", "ProfileImage", "RoleId", "UserName", "VerifyUserId", "WalletId" },
                values: new object[,]
                {
                    { new Guid("8438eac1-0ff7-455d-ae6b-32cae394373e"), null, null, null, null, null, "moderator@gmail.com", null, null, null, false, null, null, null, "$2a$11$jnq.MX2Tm.g16PTotPZTqONvf1bVyj3jfEHeWh0SqiF0oItIspheS", null, null, 2, "Moderator", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("ab65dd21-a7ac-4f46-bf84-cf36674af0e6"), null, null, null, null, null, "admin@gmail.com", null, null, null, false, null, null, null, "$2a$11$mMu9lJN3bTUsShtbQahUxeN0ZvYcGGUTPPOZrmj.Bp0b7cUedChq.", null, null, 1, "Admin", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8438eac1-0ff7-455d-ae6b-32cae394373e"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ab65dd21-a7ac-4f46-bf84-cf36674af0e6"));

            migrationBuilder.AlterColumn<bool>(
                name: "IsPriority",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDay", "CreatedBy", "CreationDate", "DeletedBy", "DeletetionDate", "Email", "FirstName", "HomeAddress", "IsBuisnessAccount", "IsDelete", "LastName", "ModificationBy", "ModificationDate", "PasswordHash", "PhoneNumber", "ProfileImage", "RoleId", "UserName", "VerifyUserId", "WalletId" },
                values: new object[,]
                {
                    { new Guid("46a64b11-6372-4d51-a995-4cc5f21290b6"), null, null, null, null, null, "moderator@gmail.com", null, null, null, false, null, null, null, "$2a$11$KX347viZ5m7g2ur3T67OIuBkBxZoM7PV3z.AKAlh.EQ1JUccF5W2y", null, null, 2, "Moderator", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("4f7f8695-f8b0-4a22-9b3d-95bf119f764d"), null, null, null, null, null, "admin@gmail.com", null, null, null, false, null, null, null, "$2a$11$J.o0TYjCyuB.zfeKLzHWAuCk4/Cy3UnqORefuw0vFOh/1z6y7q05G", null, null, 1, "Admin", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }
    }
}
