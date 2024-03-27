using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UdemyCloneBackend.Migrations
{
    /// <inheritdoc />
    public partial class fixingEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FeedbackId",
                table: "Enrollments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_FeedbackId",
                table: "Enrollments",
                column: "FeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Feedbacks_FeedbackId",
                table: "Enrollments",
                column: "FeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "FeedbackID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Feedbacks_FeedbackId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_FeedbackId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "FeedbackId",
                table: "Enrollments");
        }
    }
}
