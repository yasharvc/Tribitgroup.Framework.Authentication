using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribitgroup.Framewok.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Permissions_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_ApplicationPermission_ApplicationPermissionId",
                table: "UserPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationPermission",
                table: "ApplicationPermission");

            migrationBuilder.RenameTable(
                name: "ApplicationPermission",
                newName: "Permissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_Permissions_ApplicationPermissionId",
                table: "UserPermissions",
                column: "ApplicationPermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_Permissions_ApplicationPermissionId",
                table: "UserPermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Permissions",
                table: "Permissions");

            migrationBuilder.RenameTable(
                name: "Permissions",
                newName: "ApplicationPermission");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationPermission",
                table: "ApplicationPermission",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_ApplicationPermission_ApplicationPermissionId",
                table: "UserPermissions",
                column: "ApplicationPermissionId",
                principalTable: "ApplicationPermission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
