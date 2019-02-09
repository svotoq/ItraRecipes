using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ItraRecipes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ItraRecipes.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<UserModel> _signInManager;

        public LogoutModel(SignInManager<UserModel> signInManager)
        {
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            await _signInManager.SignOutAsync();

            return RedirectToPage("/Index");
        }
    }
}