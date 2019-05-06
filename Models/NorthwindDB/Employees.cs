using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Northwind.Models
{
    public partial class Employees
    {
        public Employees()
        {
            EmployeeTerritories = new HashSet<EmployeeTerritories>();
            InverseReportsToNavigation = new HashSet<Employees>();
            Orders = new HashSet<Orders>();
        }

        [Key]
        public int EmployeeId { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        [Required]
        public DateTime? HireDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string Extension { get; set; }
        public int? ReportsTo { get; set; }

        [NotMapped]
        public virtual Employees ReportsToNavigation { get; set; }
        [NotMapped]
        public virtual ICollection<EmployeeTerritories> EmployeeTerritories { get; set; }
        [NotMapped]
        public virtual ICollection<Employees> InverseReportsToNavigation { get; set; }
        [NotMapped]
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
