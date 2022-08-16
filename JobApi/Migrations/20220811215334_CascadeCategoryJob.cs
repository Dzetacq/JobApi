using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobApi.Migrations
{
    public partial class CascadeCategoryJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "CategoryJob_Category",
                schema: "Db",
                table: "CategoryJob");

            migrationBuilder.DropForeignKey(
                name: "CategoryJob_Job",
                schema: "Db",
                table: "CategoryJob");

            migrationBuilder.AddForeignKey(
                name: "CategoryJob_Category",
                schema: "Db",
                table: "CategoryJob",
                column: "CategoryId",
                principalSchema: "Db",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "CategoryJob_Job",
                schema: "Db",
                table: "CategoryJob",
                column: "JobId",
                principalSchema: "Db",
                principalTable: "Job",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "CategoryJob_Category",
                schema: "Db",
                table: "CategoryJob");

            migrationBuilder.DropForeignKey(
                name: "CategoryJob_Job",
                schema: "Db",
                table: "CategoryJob");

            migrationBuilder.AddForeignKey(
                name: "CategoryJob_Category",
                schema: "Db",
                table: "CategoryJob",
                column: "CategoryId",
                principalSchema: "Db",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "CategoryJob_Job",
                schema: "Db",
                table: "CategoryJob",
                column: "JobId",
                principalSchema: "Db",
                principalTable: "Job",
                principalColumn: "Id");
        }
    }
}
