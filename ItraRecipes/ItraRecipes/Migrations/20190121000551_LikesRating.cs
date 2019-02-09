using Microsoft.EntityFrameworkCore.Migrations;

namespace ItraRecipes.Migrations
{
    public partial class LikesRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Like = table.Column<bool>(nullable: false),
                    CommentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Rate = table.Column<bool>(nullable: false),
                    RecipesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Rating");
        }
    }
}
