using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Northwind.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Northwind.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeAdminController : Controller
    {
        private IUserValidator<EmployeeUser> userValidator;
        private IPasswordValidator<EmployeeUser> passwordValidator;
        private IPasswordHasher<EmployeeUser> passwordHasher;
        private UserManager<EmployeeUser> userManager;
        private INorthwindRepository repository;

        public EmployeeAdminController(UserManager<EmployeeUser> usrMgr,
            IUserValidator<EmployeeUser> userValid,
            IPasswordValidator<EmployeeUser> passValid,
            IPasswordHasher<EmployeeUser> passwordHash,
            INorthwindRepository repo)
        {
            userManager = usrMgr;
            userValidator = userValid;
            passwordValidator = passValid;
            passwordHasher = passwordHash;
            repository = repo;
        }

        public ViewResult Index() => View(new ViewEmployeesModel
        {
            EmployeeUsers = userManager.Users,
            Employees = repository.Employees
        });

        public IActionResult Register(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(EmployeeWithPassword employeeWithPassword, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Employees employee = employeeWithPassword.Employee;
                repository.AddEmployee(employee);
                if (ModelState.IsValid)
                {
                    EmployeeUser user = new EmployeeUser
                    {
                        UserName = employee.EmployeeId.ToString()
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
                            repository.DeleteEmployee(employee);
                            AddErrorsFromResult(result);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            return View("Index", new ViewEmployeesModel
            {
                EmployeeUsers = userManager.Users,
                Employees = repository.Employees
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id, string name)
        {
            var user = await userManager.FindByIdAsync(id);
            var employee = repository.Employees.SingleOrDefault(e => e.EmployeeId.ToString().Equals(name));
            if (user != null && employee != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                repository.DeleteEmployee(employee);
                if (result.Succeeded)
                {
                    return View("Index", new ViewEmployeesModel
                    {
                        EmployeeUsers = userManager.Users,
                        Employees = repository.Employees
                    });
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", new ViewEmployeesModel
            {
                EmployeeUsers = userManager.Users,
                Employees = repository.Employees
            });
        }

        public async Task<IActionResult> Edit(string id, string name)
        {
            var user = await userManager.FindByIdAsync(id);
            var employee = repository.Employees.SingleOrDefault(e => e.EmployeeId.ToString().Equals(name));
            if (user != null && employee != null)
            {
                return View(new EditEmployeeModel
                {
                    EmployeeUser = user,
                    Employee = employee
                });
            }

            return View("Index", new ViewEmployeesModel
            {
                EmployeeUsers = userManager.Users,
                Employees = repository.Employees
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditEmployeeModel editEmployeeModel)
        {
            var employeeUser = await userManager.FindByIdAsync(editEmployeeModel.EmployeeUser.Id);
            var employee = editEmployeeModel.Employee;
            if (!employee.EmployeeId.ToString().Equals(employeeUser.UserName)) return View(editEmployeeModel);
            if (!string.IsNullOrEmpty(editEmployeeModel.Password))
            {
                var validPass = await passwordValidator.ValidateAsync(userManager, employeeUser, editEmployeeModel.Password);
                if (validPass.Succeeded)
                {
                    employeeUser.PasswordHash =
                        passwordHasher.HashPassword(employeeUser, editEmployeeModel.Password);
                }
                else
                {
                    AddErrorsFromResult(validPass);
                }
            }

            repository.EditEmployee(employee);
            IdentityResult result = await userManager.UpdateAsync(employeeUser);
            if (result.Succeeded)
            {
                return View("Index", new ViewEmployeesModel
                {
                    EmployeeUsers = userManager.Users,
                    Employees = repository.Employees
                });
            }
            AddErrorsFromResult(result);

            return View(editEmployeeModel);
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
