using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobApi.Migrations
{
    public partial class CompanyFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "Db",
                table: "Company",
                type: "varchar(500)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "Db",
                table: "Company",
                type: "varchar(1000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MailAddress",
                schema: "Db",
                table: "Company",
                type: "varchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "Db",
                table: "Company",
                type: "varchar(20)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                schema: "Db",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "Db",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "MailAddress",
                schema: "Db",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "Db",
                table: "Company");
        }
    }
}
