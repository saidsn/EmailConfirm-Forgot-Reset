using JuanProject.Models;
using JuanProject.ViewModels.AccountVM;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;

using System.IO;

namespace JuanProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);

            AppUser user = new AppUser
            {
                FullName = registerVM.Fullname,
                UserName = registerVM.Username,
                Email = registerVM.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(registerVM);
            }

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string link = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token },
                          Request.Scheme,Request.Host.ToString());


            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("seidnuraliyev29@gmail.com"));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Verify email";
            string body = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/Template/Verify.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{{link}}", link);
            body = body.Replace("{{Fullname}}", user.FullName);

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("seidnuraliyev29@gmail.com", "umedzhbnlkgqrlwq");
            smtp.Send(email);
            smtp.Disconnect(true);



            return RedirectToAction(nameof(VerifyEmail));
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return NotFound();

            AppUser user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            await _userManager.ConfirmEmailAsync(user,token);

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            AppUser user = await _userManager.FindByEmailAsync(loginVM.UserNameorEmail);

            if (user == null)
            {
                user = await _userManager.FindByNameAsync(loginVM.UserNameorEmail);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Email or password wrong");
                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or password wrong!");
                return View(loginVM);
            }

            return RedirectToAction("index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM forgotPassword)
        {
            if (!ModelState.IsValid) return View();

            AppUser exsistUser = await _userManager.FindByEmailAsync(forgotPassword.Email);

            if(exsistUser == null)
            {
                ModelState.AddModelError("Email", "User not Found");
                return View();
            }


            string token = await _userManager.GeneratePasswordResetTokenAsync(exsistUser);

            string link = Url.Action(nameof(ResetPassword), "Account", new { userId = exsistUser.Id, token },
                          Request.Scheme, Request.Host.ToString());



            // create email message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("seidnuraliyev29@gmail.com"));
            email.To.Add(MailboxAddress.Parse(exsistUser.Email));
            email.Subject = "Verify password reset email";
            string body = string.Empty;

            using (StreamReader reader = new StreamReader("wwwroot/Template/Verify.html"))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{{link}}", link);
            body = body.Replace("{{Fullname}}", exsistUser.FullName);

            email.Body = new TextPart(TextFormat.Html) { Text = body };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("seidnuraliyev29@gmail.com", "umedzhbnlkgqrlwq");
            smtp.Send(email);
            smtp.Disconnect(true);

            return RedirectToAction(nameof(VerifyEmail));
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            ResetPasswordVM resetPasswordVM = new ResetPasswordVM()
            { 
                UserId = userId,
                Token = token
            };

            return View(resetPasswordVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPassword)
        {
            if (!ModelState.IsValid) return View();

            AppUser exsistUser = await _userManager.FindByIdAsync(resetPassword.UserId);

            if (exsistUser == null) return NotFound();


            if(await _userManager.CheckPasswordAsync(exsistUser,resetPassword.Password))
            {
                ModelState.AddModelError("", "This Password is your old password");
                return View(resetPassword);
            }

            await _userManager.ResetPasswordAsync(exsistUser, resetPassword.Token, resetPassword.Password);

            return RedirectToAction(nameof(Login));
        }
    }
}
