using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Northwind.Models;

namespace Northwind.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private UserManager<EmployeeUser> employeeUserManager;
        private SignInManager<EmployeeUser> employeeSignInManager;

        public AccountController(UserManager<AppUser> userMgr, SignInManager<AppUser> signInMgr, UserManager<EmployeeUser> employeeUserMgr, SignInManager<EmployeeUser> employeeSignInMgr)
        {
            userManager = userMgr;
            signInManager = signInMgr;
            employeeUserManager = employeeUserMgr;
            employeeSignInManager = employeeSignInMgr;
        }

        public IActionResult Login(string returnUrl)
        {
            // return url remembers the user's original request
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        public ViewResult AccessDenied() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel details, string returnUrl)
        {
            if (!ModelState.IsValid) return View(details);
            EmployeeUser employeeUser = null;
            AppUser user = await userManager.FindByEmailAsync(details.Login);
            if (user == null)
            {
                employeeUser = await employeeUserManager.FindByNameAsync(details.Login);
            }
            if (user != null)
            {
                await signInManager.SignOutAsync();
                await employeeSignInManager.SignOutAsync();
                var result = await signInManager.PasswordSignInAsync(user, details.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(returnUrl ?? "/");
                }
            }
            if (employeeUser != null)
            {
                await signInManager.SignOutAsync();
                await employeeSignInManager.SignOutAsync();
                var result = await employeeSignInManager.PasswordSignInAsync(employeeUser, details.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(returnUrl ?? "/");
                }
            }
            ModelState.AddModelError(nameof(LoginModel.Login), "Invalid user or password");
            return View(details);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
