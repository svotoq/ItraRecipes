using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ItraRecipes.Data;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ItraRecipes.Pages
{
    public class RecipesPageModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserModel> _userManager;
        public RecipesPageModel(
            ApplicationDbContext dbContext,
            UserManager<UserModel> userManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Enter comment")]
            [Display(Name = "Leave a comment")]
            public string Text { get; set; }
            public int RecipesId { get; set; }
        };
        public class Ingredient
        {
            public string Name { get; set; }
            public string Count { get; set; }
        };
        [BindProperty]
        public RecipesModel Recipes { get; set; }
        public List<Ingredient> ingredients { get; set; }
        public async Task<IActionResult> OnGetAsync(int RecipesId)
        {
            Recipes = await _dbContext.Recipes.FindAsync(RecipesId);
            if (Recipes == null)
            {
                return RedirectToPage("./AccessDenied");
            }
            ingredients = new List<Ingredient>();
            string[] ing = Recipes.Ingredients.Split(';');
            string[] splitIg = new string[2];
            foreach (var ig in ing)
            {
                if (ig == " ")
                {
                    break;
                }
                splitIg = ig.Split(':');
                ingredients.Add(new Ingredient { Name = splitIg[0], Count = splitIg[1] });
            }

            Recipes.Views += 1;
            await _dbContext.SaveChangesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAddComment(int RecipesId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User Not found");
            }
            var comment = new CommentsModel { UserId = user.Id, RecipesId = RecipesId, Text = Input.Text };
            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Get");
        }

        public async Task<IActionResult> OnPostLike(int CommentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (User == null)
            {
                return RedirectToPage("User Not Found");
            }
            var like = await _dbContext.Likes.Where(o => (o.UserId == user.Id) && (o.CommentId == CommentId)).AsNoTracking().FirstAsync();
            if (like == null || like.Like == true)
            {
                RedirectToAction("Get");
            }
            else
            {
                var comment = await _dbContext.Comments.Where(o => o.Id == CommentId).AsNoTracking().FirstAsync();
                if (comment == null)
                {
                    RedirectToAction("Get");
                }
                else
                {
                    comment.Likes += 1;
                    await _dbContext.SaveChangesAsync();
                }
            }
            return RedirectToAction("Get");
        }
    }
}