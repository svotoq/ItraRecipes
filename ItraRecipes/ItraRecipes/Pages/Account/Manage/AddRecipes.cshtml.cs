using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ItraRecipes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ItraRecipes.Pages.Account.Manage
{
    [Authorize]
    public class AddRecipesModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly ApplicationDbContext _dbContext;
        public AddRecipesModel(
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(60, MinimumLength = 3)]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required]
            [Display(Name = "Category")]
            public string Category { get; set; }

            [Required]
            [Display(Name = "Ingredients")]
            public string Ingredients { get; set; }

            [Required]
            [Display(Name = "Preview image")]
            public string PreviewImg { get; set; }

            [Required]
            [StringLength(600, ErrorMessage = "Description must be from {2} to {1} characters", MinimumLength = 50)]
            [Display(Name = "Description")]
            public string PreviewText { get; set; }

            [Required]
            [Display(Name = "Recipe")]
            public string Text { get; set; }

            [Required]
            [Display(Name = "Difficulty")]
            public string Difficulty { get; set; }

            [Required]
            [Display(Name = "Coocking time")]
            public string CoockingTime { get; set; }

            [Required]
            [Display(Name = "Portions")]
            [Range(1, 2000)]
            public uint Portions { get; set; }

            [ScaffoldColumn(false)]
            public string localUserId { get; set; }

            [ScaffoldColumn(false)]
            public int localRecipesId { get; set; }
        }
        public List<string> AvaliableCategories { get; private set; }
        private RecipesModel recipesToChange { get; set; }
        public async Task<IActionResult> OnGetAsync(int RecipesId, string UserId)
        {
                await CategoriesInitializeAsync();
            if (RecipesId != 0 && UserId != null)
            {
                recipesToChange = await _dbContext.Recipes.FindAsync(RecipesId);
                if (recipesToChange != null)
                {
                    var user = await _userManager.FindByIdAsync(UserId);
                    if (user != null)
                    {
                        if (!User.IsInRole("admin"))
                        {
                            if (recipesToChange.UserId != user.Id)
                            {
                                return RedirectToPage("/Account/AccessDenied");
                            }
                        }
                        Input = new InputModel
                        {
                            Name = recipesToChange.Name,
                            Category = recipesToChange.CategoryName,
                            PreviewImg = recipesToChange.PreviewImg,
                            PreviewText = recipesToChange.PreviewText,
                            Text = recipesToChange.Text,
                            Ingredients = recipesToChange.Ingredients,
                            Difficulty = recipesToChange.Difficulty,
                            CoockingTime = recipesToChange.CoockingTime,
                            Portions = recipesToChange.Portions,
                            localUserId = UserId,
                            localRecipesId = RecipesId
                        };
                    }
                }
            }
            else if (RecipesId == 0 && UserId != null)
            {
                var user = await _userManager.FindByIdAsync(UserId);
                if (user != null)
                {
                    if (!User.IsInRole("admin"))
                    {
                        if ((await _userManager.GetUserAsync(User)).Id != user.Id)
                        {
                            return RedirectToPage("/Account/AccessDenied");
                        }
                    }
                    Input = new InputModel
                    {
                        localRecipesId = 0,
                        localUserId = UserId
                    };
                }
            }
            else
            {
                Input = new InputModel
                {
                    localRecipesId = 0,
                    localUserId = null
                };
            }
            AvaliableCategories = await _dbContext.Categories.AsNoTracking().Select(o => o.Name).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            UserModel user;
            if (User.IsInRole("admin") && Input.localUserId != null)
            {
                user = await _userManager.FindByIdAsync(Input.localUserId);
            }
            else
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }

            if (user == null)
            {
                return NotFound($"User not found'{user.Id}'.");
            }
        
            if (Input.localRecipesId != 0 && Input.localUserId == user.Id)
            {
                recipesToChange = await _dbContext.Recipes.FindAsync(Input.localRecipesId);
                if (recipesToChange == null)
                {
                    return NotFound($"Recipe not found'{Input.localRecipesId}'.");
                }
                recipesToChange.Name = Input.Name;
                recipesToChange.Text = Input.Text;
                recipesToChange.UserId = Input.localUserId;
                recipesToChange.PreviewImg = Input.PreviewImg;
                recipesToChange.PreviewText = Input.PreviewText;
                recipesToChange.Date = DateTime.Now.ToShortDateString();
                recipesToChange.CategoryName = Input.Category;
                recipesToChange.Difficulty = Input.Difficulty;
                recipesToChange.CoockingTime = new string(Input.CoockingTime + "÷");
                recipesToChange.Portions = Input.Portions;
                recipesToChange.Ingredients = Input.Ingredients;
                await _dbContext.SaveChangesAsync();
                StatusMessage = "Recipe updated";
                string url = Url.Page("/Account/Manage/MyRecipes", new { UserId = user.Id });
                return Redirect(url);
            }

            var recipes = new RecipesModel { Name = Input.Name, Text = Input.Text, UserId = user.Id, PreviewImg = Input.PreviewImg, PreviewText = Input.PreviewText, Date = DateTime.Now.ToShortDateString(), CategoryName = Input.Category };
            await _dbContext.Recipes.AddAsync(recipes);
            user.Posts += 1;
            await _dbContext.SaveChangesAsync();

            StatusMessage = "Recipe added";
            return RedirectToPage("/Account/Manage/AddRecipes");
        }

        public async Task CategoriesInitializeAsync()
        {
            string[] Names = new string[] { "Soup", "Garnish", "Desert" };
            var result = await _dbContext.Categories.FirstOrDefaultAsync();
            if (result == null)
            {
                foreach (var name in Names)
                {
                    await _dbContext.Categories.AddAsync(new CategoryModel { Name = name });
                }
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}