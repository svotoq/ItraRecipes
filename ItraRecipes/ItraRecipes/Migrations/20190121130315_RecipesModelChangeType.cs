using Microsoft.EntityFrameworkCore.Migrations;

namespace ItraRecipes.Migrations
{
    public partial class RecipesModelChangeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Portions",
                table: "Recipes",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Portions",
                table: "Recipes",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
