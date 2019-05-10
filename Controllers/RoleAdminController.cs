using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Northwind.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Northwind.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleAdminController : Controller
    {
        private RoleManager<IdentityRole> appRoleManager;
        private UserManager<AppUser> appUserManager;
        private UserManager<EmployeeUser> employeeUserManager;
        private RoleStore<IdentityRole> employeeRoleStore;

        public RoleAdminController(RoleManager<IdentityRole> appRoleMgr, UserManager<AppUser> appUserMgr, UserManager<EmployeeUser> employeeUserMgr, EmployeeIdentityDbContext context)
        {
            appRoleManager = appRoleMgr;
            employeeRoleStore = new RoleStore<IdentityRole>(context);
            appUserManager = appUserMgr;
            employeeUserManager = employeeUserMgr;
        }

        public IActionResult Index() => View(appRoleManager.Roles);

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Required]string name)
        {
            IdentityResult appResult = null;
            IdentityResult employeeResult = null;

            if (ModelState.IsValid)
            {
                var appRole = await appRoleManager.FindByNameAsync(name);
                if (appRole == null)
                {
                    appResult = await appRoleManager.CreateAsync(new IdentityRole(name));
                }

                appRole = await employeeRoleStore.FindByNameAsync(name);
                if (appRole == null)
                {
                    var role = new IdentityRole(name);
                    role.NormalizedName = role.Name.ToUpper();
                    employeeResult = await employeeRoleStore.CreateAsync(role);
                    
                }
                
                if (appResult != null && !appResult.Succeeded || employeeResult != null && !employeeResult.Succeeded)
                {
                    AddErrorsFromResult(appResult);
                    AddErrorsFromResult(employeeResult);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string name)
        {
            IdentityResult appResult = null;
            IdentityResult employeeResult = null;

            IdentityRole role = await appRoleManager.FindByNameAsync(name);
            if (role != null)
            {
                 appResult = await appRoleManager.DeleteAsync(role);
            }

            role = await employeeRoleStore.FindByNameAsync(name);
            if (role != null)
            {
                employeeResult = await employeeRoleStore.DeleteAsync(role);
            }

            if (appResult != null && !appResult.Succeeded || employeeResult != null && !employeeResult.Succeeded)
            {
                AddErrorsFromResult(appResult);
                AddErrorsFromResult(employeeResult);
            }
            else
            {
                return RedirectToAction("Index");
            }

            return View("Index", appRoleManager.Roles);
        }

        public async Task<IActionResult> Edit([Required]string id)
        {
            var appRole = await appRoleManager.FindByNameAsync(id);
            var employeeRole = await employeeRoleStore.FindByNameAsync(id);
            var members = new List<AppUser>();
            var employeeMembers = new List<EmployeeUser>();
            var nonMembers = new List<AppUser>();
            var employeeNonMembers = new List<EmployeeUser>();
            foreach (var user in appUserManager.Users)
            {
                var list = await appUserManager.IsInRoleAsync(user, appRole.Name) ? members : nonMembers;
                list.Add(user);
            }
            foreach (var user in employeeUserManager.Users)
            {
                var list = await employeeUserManager.IsInRoleAsync(user, employeeRole.Name) ? employeeMembers : employeeNonMembers;
                list.Add(user);
            }
            return View(new RoleEditModel
            {
                Role = appRole,
                Members = members,
                NonMembers = nonMembers,
                EmployeeMembers = employeeMembers,
                EmployeeNonMembers = employeeNonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    var user = await appUserManager.FindByIdAsync(userId) ?? (IdentityUser) await employeeUserManager.FindByIdAsync(userId);
                    if (user == null) continue;
                    if (user.GetType() == typeof(AppUser))
                    {
                        result = await appUserManager.AddToRoleAsync((AppUser) user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                    else if (user.GetType() == typeof(EmployeeUser))
                    {
                        result = await employeeUserManager.AddToRoleAsync((EmployeeUser) user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    var user = await appUserManager.FindByIdAsync(userId) ?? (IdentityUser) await employeeUserManager.FindByIdAsync(userId);
                    if (user == null) continue;
                    if (user.GetType() == typeof(AppUser))
                    {
                        result = await appUserManager.RemoveFromRoleAsync((AppUser)user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                    else if (user.GetType() == typeof(EmployeeUser))
                    {
                        result = await employeeUserManager.RemoveFromRoleAsync((EmployeeUser)user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                    }
                }
            }
            return await Edit(model.RoleName);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
