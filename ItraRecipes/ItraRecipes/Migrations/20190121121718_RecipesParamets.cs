using Microsoft.EntityFrameworkCore.Migrations;

namespace ItraRecipes.Migrations
{
    public partial class RecipesParamets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoockingTime",
                table: "Recipes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "Recipes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Portions",
                table: "Recipes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoockingTime",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Portions",
                table: "Recipes");
        }
    }
}
