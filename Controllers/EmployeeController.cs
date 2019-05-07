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
        private UserManager<EmployeeUser> userManager;
        public EmployeeController(INorthwindRepository repo, UserManager<EmployeeUser> usrMgr)
        {
            repository = repo;
            userManager = usrMgr;
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
