using Microsoft.EntityFrameworkCore.Migrations;
using UdemyCloneBackend.Constants.RoleConstants;

#nullable disable

namespace UdemyCloneBackend.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                    table:"AspNetRoles",
                    columns: new[]{ "Id" , "Name" , "NormalizedName" , "ConcurrencyStamp"},
                    values:new object[]{Guid.NewGuid().ToString(), RoleConstants.Student , RoleConstants.Student.ToUpper() , Guid.NewGuid().ToString()}
                );
            migrationBuilder.InsertData(
                    table:"AspNetRoles",
                    columns: new[]{ "Id" , "Name" , "NormalizedName" , "ConcurrencyStamp"},
                    values:new object[]{Guid.NewGuid().ToString(), RoleConstants.Admin , RoleConstants.Admin.ToUpper() , Guid.NewGuid().ToString()}
                );
            migrationBuilder.InsertData(
                    table:"AspNetRoles",
                    columns: new[]{ "Id" , "Name" , "NormalizedName" , "ConcurrencyStamp"},
                    values:new object[]{Guid.NewGuid().ToString(), RoleConstants.Instructor , RoleConstants.Instructor.ToUpper() , Guid.NewGuid().ToString()}
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [AspNetRoles]");

        }
    }
}
