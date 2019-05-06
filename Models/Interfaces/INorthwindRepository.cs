using System.Linq;

namespace Northwind.Models
{
    public interface INorthwindRepository
    {
        IQueryable<Categories> Categories { get; }
        IQueryable<Customers> Customers { get; }
        IQueryable<Products> Products { get; }
        IQueryable<Discounts> Discounts { get; }
        IQueryable<Employees> Employees  { get; }
        IQueryable<EmployeeTerritories> EmployeeTerritories { get; }
        IQueryable<OrderDetails> OrderDetails { get; }
        IQueryable<Orders> Orders { get; }
        IQueryable<Region> Region { get; }
        IQueryable<Shippers> Shippers { get; }
        IQueryable<Territories> Territories { get; }

        void AddCustomer(Customers customer);
        void EditCustomer(Customers customer);

        void AddEmployee(Employees employee);
        void EditEmployee(Employees employee);
        void DeleteEmployee(Employees employee);

        CartItem AddToCart(CartItemJSON cartItemJSON);
    }
}
