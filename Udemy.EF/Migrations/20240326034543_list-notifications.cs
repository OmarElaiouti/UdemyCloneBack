using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UdemyCloneBackend.Migrations
{
    /// <inheritdoc />
    public partial class listnotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Notifications_NotificationID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_NotificationID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NotificationID",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "NotificationUser",
                columns: table => new
                {
                    NotificationsNotificationID = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationUser", x => new { x.NotificationsNotificationID, x.UsersId });
                    table.ForeignKey(
                        name: "FK_NotificationUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationUser_Notifications_NotificationsNotificationID",
                        column: x => x.NotificationsNotificationID,
                        principalTable: "Notifications",
                        principalColumn: "NotificationID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationUser_UsersId",
                table: "NotificationUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationUser");

            migrationBuilder.AddColumn<int>(
                name: "NotificationID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_NotificationID",
                table: "AspNetUsers",
                column: "NotificationID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Notifications_NotificationID",
                table: "AspNetUsers",
                column: "NotificationID",
                principalTable: "Notifications",
                principalColumn: "NotificationID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
