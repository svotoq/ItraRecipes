using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ItraRecipes.Data;

namespace ItraRecipes.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<UserModel> signInManager, ILogger<LoginModel> logger, UserManager<UserModel> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage ="Введите почтовый адрес")]
            [EmailAddress(ErrorMessage = "Введите почтовый адрес вида john.snow@exmaple.by")]
            public string Email { get; set; }

            [Required(ErrorMessage ="Введите пароль")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Запомнить?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        ErrorMessage = "Вы не подтвердили свой email";
                        return RedirectToAction("Get");
                    }
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return LocalRedirect(returnUrl);
                    }
                    if (result.IsLockedOut)
                    {
                        return RedirectToPage("./Lockout");
                    }
                }
                else
                {
                    ErrorMessage = "Неправильный логин и(или) пароль";
                    return RedirectToAction("Get");
                }
            }
            return RedirectToPage("Error");
        }
    }
}
