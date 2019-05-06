using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Models
{
    public class EmployeeIdentityDbContext : IdentityDbContext<EmployeeUser>
    {
        public EmployeeIdentityDbContext(DbContextOptions<EmployeeIdentityDbContext> options) : base(options)
        {

        }
    }
}
