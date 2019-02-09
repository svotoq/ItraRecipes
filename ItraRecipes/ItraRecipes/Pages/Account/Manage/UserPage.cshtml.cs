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
using Newtonsoft.Json.Linq;

namespace ItraRecipes.Pages.Account.Manage
{
    [Authorize(Roles = "admin")]
    public class UserPageModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        public UserPageModel(
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
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; }

            public string Id { get; set; }
        }
        public List<string> AvaliableRoles { get; private set; }
        private List<IdentityRole> Roles { get; set; }
        public async Task<IActionResult> OnGetAsync(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound($"Unable to load user with id '{UserId}'.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            Input = new InputModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault(),
                Id = user.Id
            };
            AvaliableRoles = await _dbContext.Roles.AsNoTracking().Select(o => o.Name).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string UserId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return NotFound($"Unable to load user with id '{UserId}'.");
            }

            if (Input.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new InvalidOperationException($"Instalation error '{user.Id}'.");
                }
            }
            if (Input.UserName != user.UserName)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    throw new InvalidOperationException($"Instalation error '{user.Id}'.");
                }
            }
            var result = await _roleManager.FindByNameAsync(Input.Role);
            if (result != null)
            {
                var userRole = await _userManager.GetRolesAsync(user);
                if (Input.Role.ToLower() != userRole.FirstOrDefault())
                {
                    if (userRole != null)
                    {
                        await _userManager.RemoveFromRoleAsync(user, userRole.FirstOrDefault());
                    }
                    await _userManager.AddToRoleAsync(user, Input.Role.ToLower());
                    if (user.UserName == User.Identity.Name)
                    {
                        await _signInManager.RefreshSignInAsync(user);
                        return RedirectToPage("/Account/Manage/Index");
                    }
                }
            }

            string url = Url.Page("/Account/Manage/UserPage", new { UserId = user.Id });
            return Redirect(url);
        }

        //public async Task<IActionResult> OnPostAsync(string id, string propertyname, string value)
        //{
        //    var status = false;
        //    var message = "";
        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user != null)
        //    {
        //        _dbContext.Entry(user).Property(propertyname).CurrentValue = value;
        //        await _dbContext.SaveChangesAsync();
        //        status = true;
        //    }
        //    else
        //    {
        //        message = "error";
        //    }

        //    var response = new { value = value, status = status, message = message };
        //    JObject o = JObject.FromObject(response);
        //    return Content(o.ToString());
        //}
    }
}