using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ItraRecipes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ItraRecipes.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<UserModel> userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [EmailAddress(ErrorMessage ="Enter email in this form john.snow@exmaple.by")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [StringLength(100, ErrorMessage = "Password must be from {2} to {1} characters.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Repeat password")]
            [Compare("Password", ErrorMessage = "Password doesn't match")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new UserModel { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "user");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code, email = Input.Email },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Email confirmation",
                        $"Confrim your email: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>confirm</a>.");

                    StatusMessage = "Confirm email";
                    return RedirectToAction("Get");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
