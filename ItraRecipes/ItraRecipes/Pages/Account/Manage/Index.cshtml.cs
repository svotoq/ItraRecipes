using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ItraRecipes.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ItraRecipes.Pages.Account.Manage
{
    [Authorize]
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            UserManager<UserModel> userManager,
            SignInManager<UserModel> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }


        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }
        }
        public static string Id { get; private set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"User not found'{_userManager.GetUserId(User)}'.");
            }
            Id = user.Id;
            var userName = user.UserName;
            var email = user.Email;


            Input = new InputModel
            {
                Email = email,
                UserName = userName
            };

            IsEmailConfirmed = user.EmailConfirmed;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"User not found '{_userManager.GetUserId(User)}'.");
            }

            if(Input.UserName != user.UserName)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.UserName);
                if(!setUserNameResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or already taken");
                    return Page();
                }
            }
            var userRole = await _userManager.GetRolesAsync(user);
            if(userRole.FirstOrDefault() == "admin")
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or already taken");
                    return Page();
                }
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _userManager.ConfirmEmailAsync(user, token);
                await _signInManager.RefreshSignInAsync(user);

                StatusMessage = "User profile updated";

                return RedirectToPage();
            }
            if (Input.Email != user.Email)
            {
                StatusMessage = "Check your email adress to confirm";
                await OnPostSendVerificationEmailAsync();
            }
            else
            {
                StatusMessage = "User profile updated";
            }
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Cannot load user with id '{_userManager.GetUserId(User)}'.");
            }


            var userId = user.Id;
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code, email = Input.Email },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                Input.Email,
                "Submit",
                $"Submit changes: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Submit</a>.");

            StatusMessage = "Accept changes on your email.";
            return RedirectToPage();
        }
    }
}
