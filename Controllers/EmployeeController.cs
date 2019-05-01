using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Northwind.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Northwind.Controllers
{
    public class EmployeeController : Controller
    {
        // this controller depends on the NorthwindRepository & the UserManager
        private INorthwindRepository repository;
        private UserManager<AppUser> userManager;
        public EmployeeController(INorthwindRepository repo, UserManager<AppUser> usrMgr)
        {
            repository = repo;
            userManager = usrMgr;
        }

        //TODO: Create view
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> Register(EmployeeWithPassword employeeWithPassword)
        {
            if (ModelState.IsValid)
            {
                Employees employee = employeeWithPassword.Employee;
                if (ModelState.IsValid)
                {
                    AppUser user = new AppUser
                    {
                        //concatenate first letter of first name with last name for a unique (for now) user name
                        UserName = employee.FirstName[0] + employee.LastName
                    };
                    // Add user to Identity DB
                    IdentityResult result = await userManager.CreateAsync(user, employeeWithPassword.Password);
                    if (!result.Succeeded)
                    {
                        AddErrorsFromResult(result);
                    }
                    else
                    {
                        // Assign user to employee Role
                        result = await userManager.AddToRoleAsync(user, "Employee");

                        if (!result.Succeeded)
                        {
                            // Delete User from Identity DB
                            await userManager.DeleteAsync(user);
                            AddErrorsFromResult(result);
                        }
                        else
                        {
                            // Create employee (Northwind)
                            repository.AddEmployee(employee);
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            return View();
        }

        //TODO: Create Views
        [Authorize(Roles = "Employee")]
        public IActionResult Account() => View(repository.Employees.FirstOrDefault(e => e.FirstName[0] + e.LastName == User.Identity.Name));

        [Authorize(Roles = "Employee"), HttpPost, ValidateAntiForgeryToken]
        public IActionResult Account(Employees employees)
        {
            // Edit customer info
            repository.EditEmployee(employees);
            return RedirectToAction("Index", "Home");
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
