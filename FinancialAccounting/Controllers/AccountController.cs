using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FinancialAccountingConstruction.DAL;
using FinancialAccountingConstruction.DAL.Models.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using FinancialAccounting.Models;

namespace FinancialAccounting.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult ManageAccounts()
        {
            var applicationDbContext = new ApplicationDbContext();
            var currentUserId = User.Identity.GetUserId();
            var users = applicationDbContext.Users.Where(us => us.Id != currentUserId).ToList();
            return View(users);
        }

        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.Role.ToString());

                    return RedirectToAction("ManageAccounts", "Account");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        

        //
        // GET: /Account/Manage
        public ActionResult Remove(Guid? userId)
        {
            if (userId != null)
            {
                var uId = userId.ToString();
                using (var context = new ApplicationDbContext())
                {
                    context.Users.Remove(context.Users.Single(user => user.Id == uId));
                    var roles = context.Users
                                 .Where(u => u.Id == uId)
                                 .SelectMany(u => u.Roles)
                                 .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r);

                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(uId, role.Name);
                    }

                    context.SaveChanges();
                }

                ViewBag.ReturnUrl = Url.Action("Manage");
            }

            return RedirectToAction("ManageAccounts", "Account");
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(Guid? userId)
        {
            if (userId != null)
            {
                var user = UserManager.FindById(userId.ToString());
                var viewModel = GenerateViewModel(userId, user);

                ViewBag.Title = string.Format("Пользователь {0}", user.UserName);
                ViewBag.HasLocalPassword = HasPassword();
                ViewBag.ReturnUrl = Url.Action("Manage");

                return View(viewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            if (ModelState.IsValid)
            {

                using (var context = new ApplicationDbContext())
                {
                    var uId = model.UserId.ToString();

                    var currentUser = context.Users.Single(u => u.Id == uId);

                    currentUser.UserName = model.UserName;

                    context.Entry(currentUser).State = EntityState.Modified;

                    var roles = context.Users
                                .Where(u => u.Id == uId)
                                .SelectMany(u => u.Roles)
                                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r);

                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(currentUser.Id, role.Name);
                    }

                    context.SaveChanges();
                    UserManager.AddToRole(currentUser.Id, model.Role.ToString());
                }

                return RedirectToAction("ManageAccounts", "Account");

            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        private ManageUserViewModel GenerateViewModel(Guid? userId, ApplicationUser user)
        {
            var context = new ApplicationDbContext();
            var uId = userId.ToString();
            var roles = context.Users
                                .Where(u => u.Id == uId)
                                .SelectMany(u => u.Roles)
                                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                                .FirstOrDefault();

            var viewModel = new ManageUserViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                Role = (UserRoles)Convert.ToInt32(roles.Id)
            };

            return viewModel;
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}