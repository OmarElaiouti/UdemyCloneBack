using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UdemyCloneBackend.Migrations
{
    /// <inheritdoc />
    public partial class listrequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objective_Courses_CourseID",
                table: "Objective");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Objective",
                table: "Objective");

            migrationBuilder.DropColumn(
                name: "Requirements",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "Objective",
                newName: "Objectives");

            migrationBuilder.RenameIndex(
                name: "IX_Objective_CourseID",
                table: "Objectives",
                newName: "IX_Objectives_CourseID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Objectives",
                table: "Objectives",
                column: "ObjectiveID");

            migrationBuilder.CreateTable(
                name: "Requirements",
                columns: table => new
                {
                    RequirementID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requirements", x => x.RequirementID);
                    table.ForeignKey(
                        name: "FK_Requirements_Courses_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requirements_CourseID",
                table: "Requirements",
                column: "CourseID");

            migrationBuilder.AddForeignKey(
                name: "FK_Objectives_Courses_CourseID",
                table: "Objectives",
                column: "CourseID",
                principalTable: "Courses",
                principalColumn: "CourseID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objectives_Courses_CourseID",
                table: "Objectives");

            migrationBuilder.DropTable(
                name: "Requirements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Objectives",
                table: "Objectives");

            migrationBuilder.RenameTable(
                name: "Objectives",
                newName: "Objective");

            migrationBuilder.RenameIndex(
                name: "IX_Objectives_CourseID",
                table: "Objective",
                newName: "IX_Objective_CourseID");

            migrationBuilder.AddColumn<string>(
                name: "Requirements",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Objective",
                table: "Objective",
                column: "ObjectiveID");

            migrationBuilder.AddForeignKey(
                name: "FK_Objective_Courses_CourseID",
                table: "Objective",
                column: "CourseID",
                principalTable: "Courses",
                principalColumn: "CourseID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
