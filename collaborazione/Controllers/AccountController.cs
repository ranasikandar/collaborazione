using collaborazione.Models;
using collaborazione.Utilities;
using collaborazione.Utilities.Email;
using collaborazione.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.Controllers
{
    public class AccountController:Controller
    {
        #region CTOR
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private IViewToStringService viewToStringService;
        private readonly IEmailService emailService;
        private readonly IConfiguration config;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager
            , IViewToStringService _viewToStringService, IEmailService _emailService, IConfiguration _configuration, SignInManager<ApplicationUser> _signInManager)
        {
            this.userManager = _userManager;
            roleManager = _roleManager;
            viewToStringService = _viewToStringService;
            emailService = _emailService;
            config = _configuration;
            signInManager = _signInManager;
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            ////read cookie from IHttpContextAccessor  
            //string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies["key"];
            ////read cookie from Request object  
            string cookieValueFromReq2 = Request.Cookies["inviteByFrn"];

            string cookieValueFromReq = HttpContext.Request.Cookies["inviteByFrn"];
             
            RegisterViewModel vm = new RegisterViewModel();
            vm.InvitedBy = cookieValueFromReq;

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Name = model.NameR,
                    UserName = model.EmailR,
                    Email = model.EmailR,
                    InvitedBy=model.InvitedBy
                };

                IdentityResult result = await userManager.CreateAsync(user, model.PasswordR);

                if (result.Succeeded)
                {
                    string token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    string confirmationLink = Url.Action("ConfirmEmail", "Account",
                                            new { userId = user.Id, token = token }, Request.Scheme);

                    ApplicationUser _user = await userManager.FindByEmailAsync(user.Email);

                    //role asign
                    IdentityRole role = await roleManager.FindByNameAsync("user");
                    IdentityResult roleResult = await userManager.AddToRoleAsync(_user, role.Name);
                    //role asign end

                    string body = await viewToStringService.RenderToStringAsync("~/Views/EmailTemplate/TmpEmailVerification.cshtml"
                        , new EmailVerificationTemplateModel
                        {
                            Email = _user.Email,
                            UserName = _user.Name,
                            Url = confirmationLink
                        ,
                            Text = "Verify Your Email",
                            UserId = _user.Id
                        });

                    bool isSent = await emailService.SendEmailAsync(config["AppName"], config["SupportEmail"], _user.Name, _user.Email, "Please Verify Your Email Address", body);

                    if (isSent)
                    {
                        TempData["RegSuccTitleMessage"] = "Registration Successful";
                        TempData["RegSuccDetailMessage"] = "Before you can Login, please confirm your " +
                            "email, by clicking on the confirmation link we have sent an email to you at " + _user.Email;
                    }
                    else
                    {
                        TempData["RegSuccTitleMessage"] = "Registration Successful";
                        TempData["RegSuccDetailMessage"] = "We tried to send you Email confirmation link to " + _user.Email + " but it failed.";
                    }

                    //ViewBag.JavaScriptFunction = $"window.location.replace('{Url.Action("Register_Success", "Account")}');";
                    return View("Register_Success");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register_Success()
        {
            if (TempData["RegSuccTitleMessage"] != null)
            {
                ViewBag.TitleMessage = TempData["RegSuccTitleMessage"].ToString();
            }
            if (TempData["RegSuccDetailMessage"] != null)
            {
                ViewBag.DetailMessage = TempData["RegSuccDetailMessage"].ToString();
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            TempData["RegSuccTitleMessage"] = null;
            TempData["RegSuccDetailMessage"] = null;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                //return RedirectToAction("Register_Success", "Account");
                Response.StatusCode = 400;
                ViewBag.ErrorTitle = "Invalid Request";
                ViewBag.ErrorMessage = "Invalid Parameters of Confirm Email";
                return View("Error500");
            }

            ApplicationUser user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                //ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                Response.StatusCode = 400;
                ViewBag.ErrorTitle = "Invalid Request";
                ViewBag.ErrorMessage = "User not found";
                return View("Error500");
            }

            IdentityResult result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                TempData["ViewMsgAlert"] = "Your Email Verified, Please Login";
                return RedirectToAction("Login", "Account");
                //dont signin auto
            }
            else
            {
                ViewBag.ErrorTitle = "Email Verification failed";
                ViewBag.ErrorMessage = "Email Cannot be Verified";
                return View("Error500");
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string ReturnUrl)
        {
            if (TempData["ViewMsgAlert"] != null&&!string.IsNullOrEmpty(TempData["ViewMsgAlert"].ToString()))
            {
                ViewBag.ViewMsgAlert = TempData["ViewMsgAlert"].ToString();
            }

            LoginViewModel model = new LoginViewModel
            {
                ReturnUrlL = ReturnUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl)
        {
            TempData["ViewMsgAlert"] = null;

            model.ReturnUrlL = ReturnUrl;

            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByEmailAsync(model.EmailL);

                if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.PasswordL)))
                {
                    ModelState.AddModelError(string.Empty, "Email Not Verified Yet");
                    return View(model);
                }

                if (user != null)
                {
                    //check if blocked or deleted before login
                    if ((!(user.IsDeleted ?? false)))
                    {
                        var result = await signInManager.PasswordSignInAsync(user, model.PasswordL, model.RememberMe, true);

                        if (result.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                            {
                                //ViewBag.JavaScriptFunction = $"window.location.replace('{ReturnUrl}');";
                                //return View(model);
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                //ROLE BASED REDIRECT FOR SITE USERS

                                IdentityRole role = await roleManager.FindByNameAsync("admin");
                                if (await userManager.IsInRoleAsync(user, role.Name))
                                {
                                    //ViewBag.JavaScriptFunction = $"window.location.replace('{Url.Action("Index", "Admin")}');";
                                    return RedirectToAction("ClientsStatus", "User");
                                }

                                role = await roleManager.FindByNameAsync("user");
                                if (await userManager.IsInRoleAsync(user, role.Name))
                                {
                                    //ViewBag.JavaScriptFunction = $"window.location.replace('{Url.Action("Index", "User")}');";
                                    return RedirectToAction("Index", "User");
                                }

                                //return PartialView("Account/_Login", model);
                            }
                        }
                        else
                        {
                            //invalid password or couldn't signin the user
                            ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                        }

                        if (result.IsLockedOut)
                        {
                            //ViewBag.ErrorTitle = "Sorry Your Account has been Locked";
                            //ViewBag.ErrorMessage = "Account Locked could be Temporary please try again later after some time";
                            //return View("AccountLocked"); 
                            ViewBag.JavaScriptFunction = $"window.location.replace('{Url.Action("AccountLocked", "Account")}');";
                        }
                        if (result.IsNotAllowed)
                        {
                            ModelState.AddModelError(string.Empty, "Login Not Allowed");
                        }

                    }
                    else
                    {
                        //check if blocked or deleted before login
                        ModelState.AddModelError(string.Empty, "Login Not Allowed");
                        //ViewBag.JavaScriptFunction = $"window.location.replace('{Url.Action("AccountLocked", "Account")}');";
                        return View("~/Views/Account/AccountLocked.cshtml");
                    }
                }
                else
                {
                    //user not found with email
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                }

            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            string _message = string.Empty;

            if (ModelState.IsValid)
            {
                ApplicationUser _user = await userManager.FindByEmailAsync(model.Email);

                if (_user != null)//&& await userManager.IsEmailConfirmedAsync(user)
                {
                    string token = await userManager.GeneratePasswordResetTokenAsync(_user);

                    string passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    string body = await viewToStringService.RenderToStringAsync("~/Views/EmailTemplate/TmpForgotPassword.cshtml"
                        , new ForgotPasswordTemplateModel
                        {
                            Email = _user.Email,
                            UserName = _user.Name,
                            Url = passwordResetLink,
                            Text = "Reset Your Password",
                            UserId = _user.Id
                        });

                    bool isSent = await emailService.SendEmailAsync(config["AppName"], config["SupportEmail"], _user.Name, _user.Email, "Password Reset", body);
                    if (isSent)
                    {
                        _message = "We have sent an email with the instructions to reset your password.";
                    }
                    else
                    {
                        _message = "We tried to send you an email with the instructions to reset your password. unfortunately it faild, please try again.";
                    }
                }
                else
                {
                    _message = "If you have an account with us, we have sent an email with the instructions to reset your password.";
                }
            }

            ViewBag.Message = _message;
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                Response.StatusCode = 400;
                ViewBag.ErrorTitle = "Invalid Request";
                ViewBag.ErrorMessage = "Invalid Password Reset Parameters";
                return View("Error500");
            }
            else
            {
                return View();
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    IdentityResult result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        //LOCKOUT WILL BE USED BY ADMIN TO DISABLE USERS// OR SIMPLE DELETE EMAIL AND PASSWORD TO DELETE USER
                        //if (await userManager.IsLockedOutAsync(user))
                        //{
                        //    await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        //}

                        String _messageAlert;

                        if (await userManager.IsLockedOutAsync(user))
                        {
                            _messageAlert = "Your Password has been reset But your accouct is Locked. if lockout was temporary, login with your new password after some time.";
                        }
                        else
                        {
                            _messageAlert = "Your Password has been reset Please Login with your new Password";
                        }

                        //if (!user.EmailConfirmed)//maybe social login user is trying to reset its password so also verify its email
                        //{
                        //    IdentityResult emailConfirmResult = await userManager.ConfirmEmailAsync(user, await userManager.GenerateEmailConfirmationTokenAsync(user));
                        //}

                        TempData["ViewMsgAlert"] = _messageAlert;
                        return RedirectToAction("Login", "Account");
                    }

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
                else
                {
                    Response.StatusCode = 400;
                    //ModelState.AddModelError("", "Invalid Password Reset Request, User not found");
                    ViewBag.ErrorTitle = "Invalid Request";
                    ViewBag.ErrorMessage = "Invalid Password Reset Request, User not found";
                    return View("Error500");
                }

            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser currentUser = await userManager.GetUserAsync(HttpContext.User);
                IdentityResult status = await userManager.ChangePasswordAsync(currentUser, model.OldPassword, model.ConfirmPassword);

                if (status.Succeeded)
                {
                    await signInManager.SignOutAsync();

                    TempData["ViewMsgAlert"] = "Password has been Changed Login again with your new Password";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (IdentityError error in status.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            ViewBag.ErrorTitle = "Sorry Access Denied";
            ViewBag.ErrorMessage = "You may not have permission to view this resource";
            return View("~/Views/Account/AccessDenied.cshtml");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccountLocked()
        {
            ////OPTINAL
            ViewBag.ErrorTitle = "Your Account is Locked";
            ViewBag.ErrorMessage = "this lock could be temporary or permanently if you could not login to your account after some time please contact support person of this website";

            return View();
        }

        [Route("Logout")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            TempData["ViewMsgAlert"] = "You have been Logout";
            return RedirectToAction("Login", "Account");
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string emailR)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(emailR);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {emailR} is already in use");
            }
        }

    }
}
