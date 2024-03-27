using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UdemyCloneBackend.Migrations
{
    /// <inheritdoc />
    public partial class listobjectives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Objectives",
                table: "Courses");

            migrationBuilder.CreateTable(
                name: "Objective",
                columns: table => new
                {
                    ObjectiveID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objective", x => x.ObjectiveID);
                    table.ForeignKey(
                        name: "FK_Objective_Courses_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Objective_CourseID",
                table: "Objective",
                column: "CourseID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Objective");

            migrationBuilder.AddColumn<string>(
                name: "Objectives",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
