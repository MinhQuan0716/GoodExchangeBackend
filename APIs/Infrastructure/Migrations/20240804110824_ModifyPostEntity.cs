using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyPostEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3d467fb4-a342-4fe1-b05b-563349065dd4"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8638a4cb-117a-4e7b-b8e6-157188db8f63"));

            migrationBuilder.AddColumn<bool>(
                name: "IsPriority",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Posts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDay", "CreatedBy", "CreationDate", "DeletedBy", "DeletetionDate", "Email", "FirstName", "HomeAddress", "IsBuisnessAccount", "IsDelete", "LastName", "ModificationBy", "ModificationDate", "PasswordHash", "PhoneNumber", "ProfileImage", "RoleId", "UserName", "VerifyUserId", "WalletId" },
                values: new object[,]
                {
                    { new Guid("2dcd03c5-396c-4862-8096-314d27bd5407"), null, null, null, null, null, "moderator@gmail.com", null, null, null, false, null, null, null, "$2a$11$3rCPl/ZRLYetlMOAwaLcpOsmTXkFiVvYXGZYpwRTPKgOfDB4opUbW", null, null, 2, "Moderator", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("d98048da-4376-4a58-ba02-e16384f5858a"), null, null, null, null, null, "admin@gmail.com", null, null, null, false, null, null, null, "$2a$11$SNNORHaVZ1g6EdzZ3oVjEeueettbg1Xw16J8RbhybuQ/PJxTMjNFm", null, null, 1, "Admin", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_UserId",
                table: "Posts");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2dcd03c5-396c-4862-8096-314d27bd5407"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d98048da-4376-4a58-ba02-e16384f5858a"));

            migrationBuilder.DropColumn(
                name: "IsPriority",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Posts");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDay", "CreatedBy", "CreationDate", "DeletedBy", "DeletetionDate", "Email", "FirstName", "HomeAddress", "IsBuisnessAccount", "IsDelete", "LastName", "ModificationBy", "ModificationDate", "PasswordHash", "PhoneNumber", "ProfileImage", "RoleId", "UserName", "VerifyUserId", "WalletId" },
                values: new object[,]
                {
                    { new Guid("3d467fb4-a342-4fe1-b05b-563349065dd4"), null, null, null, null, null, "admin@gmail.com", null, null, null, false, null, null, null, "$2a$11$soYs4e4aFsfmbHXX613q/u1oTQYlxHUda3G0M0RZBrH0wVS9n/W36", null, null, 1, "Admin", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("8638a4cb-117a-4e7b-b8e6-157188db8f63"), null, null, null, null, null, "moderator@gmail.com", null, null, null, false, null, null, null, "$2a$11$I0ul7peKTWUXv5FUcvtCeOAP/28OOlrMSm4V7mC0Obj/1/l/oZLf2", null, null, 2, "Moderator", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }
    }
}
