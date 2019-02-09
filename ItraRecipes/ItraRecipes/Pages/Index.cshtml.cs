using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItraRecipes.Data;
using ItraRecipes.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ItraRecipes.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string NameSort { get; set; }

        public string DateSort { get; set; }

        public string ViewsSort { get; set; }

        public string CurrentSort { get; set; }

        public List<RecipesModel> MostRatedRecipes { get; set; }

        public PaginateList<RecipesModel> Recipes { get; private set; }

        public async Task OnGetAsync(string sortOrder, int? pageIndex)
        {
            NameSort = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            DateSort = sortOrder == "Date" ? "Date_desc" : "Date";
            ViewsSort = sortOrder == "Views" ? "Views_desc" : "Views";
            IQueryable<RecipesModel> recipes = from s in _dbContext.Recipes
                                               select s;

            switch (sortOrder)
            {
                case "Name_desc":
                    recipes = recipes.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    recipes = recipes.OrderBy(s => s.Date);
                    break;
                case "Date_desc":
                    recipes = recipes.OrderByDescending(s => s.Date);
                    break;
                case "Views":
                    recipes = recipes.OrderBy(s => s.Views);
                    break;
                case "Views_desc":
                    recipes = recipes.OrderByDescending(s => s.Views);
                    break;
                default:
                    recipes = recipes.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 3;
            Recipes = await PaginateList<RecipesModel>.CreateAsync(
                recipes.AsNoTracking(), pageIndex ?? 1, pageSize) ;
        }
    }
}
