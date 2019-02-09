using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ItraRecipes.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<RecipesModel> Recipes { get; set; }
        public DbSet<CommentsModel> Comments { get; set; }
        public DbSet<LikesModel> Likes { get; set; }
        public DbSet<RatingModel> Rating { get; set; }
    }
}
