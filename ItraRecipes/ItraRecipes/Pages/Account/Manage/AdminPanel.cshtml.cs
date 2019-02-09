using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ItraRecipes.Data;
using Microsoft.EntityFrameworkCore;
using ItraRecipes.Services;

namespace ItraRecipes.Pages.Account.Manage
{
    [Authorize(Roles = "admin")]
    public class AdminPanel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        public AdminPanel(
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            ApplicationDbContext dbContext,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _roleManager = roleManager;
        }
        public PaginateList<UserModel> Persons { get; private set; }

        public string NameSort { get; set; }
        public string PostSort { get; set; }
        public string RateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }


        public async Task OnGetAsync(string sortOrder, string searchString, int? pageIndex)
        {
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            PostSort = sortOrder == "Post" ? "Post_desc" : "Post";
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

            IQueryable<UserModel> users = from s in _dbContext.Users
                                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.UserName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(s => s.UserName);
                    break;
                case "Post":
                    users = users.OrderBy(s => s.Posts);
                    break;
                case "Post_desc":
                    users = users.OrderByDescending(s => s.Posts);
                    break;
                case "Rate":
                    users = users.OrderBy(s => s.UserRating);
                    break;
                case "Rate_desc":
                    users = users.OrderByDescending(s => s.UserRating);
                    break;
                default:
                    users = users.OrderBy(s => s.UserName);
                    break;
            }
            int pageSize = 15;
            Persons = await PaginateList<UserModel>.CreateAsync(
            users.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
        public async Task<IActionResult> OnPostDeleteAsync(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                if(user.UserName == User.Identity.Name)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToPage("/Index");
                }
            }

            return RedirectToPage("/Account/Manage/AdminPanel");
        }
        public async Task<IActionResult> OnPostLockoutAsync(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if(user != null)
            {
                if (user.LockoutEnd == null)
                {
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                }
                else
                {
                    await _userManager.SetLockoutEndDateAsync(user, null);
                }
                if(user.UserName == User.Identity.Name)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToPage("/Account/Lockout");
                }
            }
            return RedirectToPage("/Account/Manage/AdminPanel");
        }
       
    }
}