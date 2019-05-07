﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Northwind.Models
{
    public class CustomerWithPassword
    {
        public Customers Customer { get; set; }
        [UIHint("password"), Required]
        public string Password { get; set; }
    }

    public class EmployeeWithPassword
    {
        public Employees Employee { get; set; }
        [UIHint("password"), Required]
        public string Password { get; set; }
    }

    public class LoginModel
    {
        [Required]
        public string Login { get; set; }

        [Required, UIHint("password")]
        public string Password { get; set; }
    }

    public class ViewEmployeesModel
    {
        public IEnumerable<EmployeeUser> EmployeeUsers { get; set; }
        public IEnumerable<Employees> Employees { get; set; }
    }

    public class EditEmployeeModel
    {
        public EmployeeUser EmployeeUser { get; set; }
        public Employees Employee { get; set; }
        public string Password { get; set; }
    }

    public class CreateModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
	
	public class RoleEditModel
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<EmployeeUser> EmployeeMembers { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
        public IEnumerable<EmployeeUser> EmployeeNonMembers { get; set; }
    }

    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public string RoleId { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}
