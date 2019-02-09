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

namespace ItraRecipes.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<UserModel> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !user.EmailConfirmed)
                {
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code, email = user.Email },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Восстановить пароль</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
