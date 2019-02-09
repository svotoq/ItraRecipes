using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItraRecipes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ItraRecipes.Services;

namespace ItraRecipes.Pages.Account.Manage
{
    [Authorize]
    public class MyRecipesModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserModel> _userManager;
        public MyRecipesModel(
            ApplicationDbContext dbContext,
            UserManager<UserModel> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public string NameSort { get; set; }
        public string CategorySort { get; set; }
        public string ingredientsSort { get; set; }
        public string DateSort { get; set; }
        public string RateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public PaginateList<RecipesModel> Recipes { get; private set; }
        public string Id { get; private set; }

        public async Task<IActionResult> OnGetAsync(string UserId, string sortOrder, string searchString, int? pageIndex)
        {
            UserModel user;
            if (UserId == null)
            {
                user = await _userManager.GetUserAsync(User);
            }
            else
            {
                user = await _userManager.FindByIdAsync(UserId);
            }
            if (user == null)
            {
                return RedirectToPage("/Error");
            }
            if (User.Identity.Name != user.UserName && !User.IsInRole("admin"))
            {
                return RedirectToPage("Account/AccessDenied");
            }
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            CategorySort = sortOrder == "Category" ? "Category_desc" : "Category";
            ingredientsSort = sortOrder == "ingredients" ? "ingredients_desc" : "ingredients";
            DateSort = sortOrder == "Date" ? "Date_desc" : "Date";
            RateSort = sortOrder == "Rate" ? "Rate_desc" : "Rate";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = CurrentFilter;
            }
            CurrentFilter = searchString;
            IQueryable<RecipesModel> recipes = from s in _dbContext.Recipes
                                               select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                recipes = recipes.Where(s => s.Name.Contains(searchString)
                                       || s.CategoryName.Contains(searchString)
                                       || s.Date.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    recipes = recipes.OrderByDescending(s => s.Name);
                    break;
                case "Category":
                    recipes = recipes.OrderBy(s => s.CategoryName);
                    break;
                case "Category_desc":
                    recipes = recipes.OrderByDescending(s => s.CategoryName);
                    break;
                case "ingredients":
                    recipes = recipes.OrderBy(s => s.Ingredients);
                    break;
                case "ingredients_desc":
                    recipes = recipes.OrderByDescending(s => s.Ingredients);
                    break;
                case "Date":
                    recipes = recipes.OrderBy(s => s.Date);
                    break;
                case "Date_desc":
                    recipes = recipes.OrderByDescending(s => s.Date);
                    break;
                case "Rate":
                    recipes = recipes.OrderBy(s => s.Rating);
                    break;
                case "Rate_desc":
                    recipes = recipes.OrderByDescending(s => s.Rating);
                    break;
                default:
                    recipes = recipes.OrderBy(s => s.Name);
                    break;
            }
            Id = user.Id;
            int pageSize = 3;
            Recipes = await PaginateList<RecipesModel>.CreateAsync(
                recipes.Where(o=>o.UserId==user.Id).AsNoTracking(), pageIndex ?? 1, pageSize);
            return Page();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int RecipesId, string UserId)
        {
            var recipes = await _dbContext.Recipes.FindAsync(RecipesId);
            if (recipes != null)
            {
                var user = await _userManager.FindByIdAsync(UserId);
                if (user != null)
                {
                    _dbContext.Recipes.Remove(recipes);
                    user.Posts -= 1;
                    await _dbContext.SaveChangesAsync();
                    string url = Url.Page("/Account/Manage/MyRecipes", new { UserId = UserId });
                    return Redirect(url);
                }
            }
            return RedirectToPage("./Error");
        }
    }
}