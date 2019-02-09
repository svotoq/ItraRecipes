using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItraRecipes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ItraRecipes.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;

        public ConfirmEmailModel(UserManager<UserModel> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code, string email)
        {
            if (userId == null || code == null || email == null)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user wirh id'{userId}'.");
            }
            var results = await _userManager.ConfirmEmailAsync(user, code);
            if (!results.Succeeded)
            {
                ErrorMessage = "Installation error";
                return Page();
            }

            if (user.Email != email)
            {
                var setNewEmail = await _userManager.SetEmailAsync(user, email);
                if(!setNewEmail.Succeeded)
                {
                    ErrorMessage = "Installation error";
                    return Page();
                }
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);
               
                StatusMessage = "New email installed";
                return Page();
            }

            
            StatusMessage = "Thank you for confirming.";
            return Page();
        }
    }
}
