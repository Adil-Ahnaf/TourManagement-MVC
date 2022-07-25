using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourManagemantSystem.Data.ChangedObjects;
using TourManagemantSystem.Helpers;
using TourManagemantSystem.Models;

namespace TourManagemantSystem.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly UserManager<TourUser> userManager;
        private readonly SignInManager<TourUser> signInManager;
        private readonly IEmailHelper emailHelper;

        public AccountController(UserManager<TourUser> userManager, SignInManager<TourUser> signInManager, IEmailHelper emailHelper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailHelper = emailHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserSignUp(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Register", model);

            var user = new TourUser
            {
                UserName = model.Name,
                Email = model.Email,
                Address = model.Address,
                PhoneNumber = model.Phone,
            };
            var result = await userManager.CreateAsync(user, model.Password); 
            if (result.Succeeded)
            {
                var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.ActionLink("ConfirmEmail", values: new { userId = user.Id, token = confirmationToken });

                await emailHelper.SendAsync("learners.acdemy123@gmail.com",
                    user.Email,
                    "Email Confirmation",
                    $"Please click this link to veirfy the email address {confirmationLink}", null
                    );

                //return RedirectToAction("ConfirmEmail", new { userId = user.Id, token = confirmationToken });
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("Register", item.Description);
                }
                return View("Register", model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await this.userManager.ConfirmEmailAsync(user, token);
                ViewBag.Message = "Email verified!";
            }
            else
            {
                ViewBag.Message = "User not found!";
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ValidateLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userObj = await userManager.FindByEmailAsync(model.Email); /*Finding by email but passing user object for more detail check defination*/
                var signInresult = await signInManager.PasswordSignInAsync(userObj, model.Password, model.IsPersistentCookie, true);
               
                //validate the credential
                if (signInresult.Succeeded)
                {
                    //var user = await userManager.FindByNameAsync(model.UserName);
                    //var userClaims = await userManager.GetClaimsAsync(user);
                    return Redirect("~/Package/Index");
                }
                else
                {
                    if (signInresult.IsLockedOut)
                    {
                        ModelState.AddModelError("Login", "You are locked out.");
                        return View("Login", model);
                    }
                    else
                    {
                        ModelState.AddModelError("Login", "Login Failed.");
                        return View("Login", model);
                    }
                }
            }
            else
            {
                return View("Login", model);
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordSubmit(ChangePasswordViewModel model)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            return RedirectToAction("Logout");
        }
    }
}
