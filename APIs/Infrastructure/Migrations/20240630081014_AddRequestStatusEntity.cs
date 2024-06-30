using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestStatusEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestStatus",
                table: "Requests");

            migrationBuilder.AddColumn<int>(
                name: "RequestStatusId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RequestStatuses",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatuses", x => x.StatusId);
                });

            migrationBuilder.InsertData(
                table: "RequestStatuses",
                columns: new[] { "StatusId", "StatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Accept" },
                    { 3, "Reject" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestStatusId",
                table: "Requests",
                column: "RequestStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_RequestStatuses_RequestStatusId",
                table: "Requests",
                column: "RequestStatusId",
                principalTable: "RequestStatuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_RequestStatuses_RequestStatusId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "RequestStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestStatusId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestStatusId",
                table: "Requests");

            migrationBuilder.AddColumn<bool>(
                name: "RequestStatus",
                table: "Requests",
                type: "bit",
                nullable: true);
        }
    }
}
