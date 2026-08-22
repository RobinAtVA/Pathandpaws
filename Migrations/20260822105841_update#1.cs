using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathAndPaws.Migrations
{
    /// <inheritdoc />
    public partial class update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Leads",
                newName: "Website");

            migrationBuilder.RenameColumn(
                name: "Company",
                table: "Leads",
                newName: "OwnersName");

            migrationBuilder.AddColumn<string>(
                name: "DogsName",
                table: "Leads",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DogsName",
                table: "Leads");

            migrationBuilder.RenameColumn(
                name: "Website",
                table: "Leads",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "OwnersName",
                table: "Leads",
                newName: "Company");
        }
    }
}
