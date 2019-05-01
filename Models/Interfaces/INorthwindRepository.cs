using System.Linq;

namespace Northwind.Models
{
    public interface INorthwindRepository
    {
        //jeff models
        IQueryable<Category> Categories { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<Product> Products { get; }
        IQueryable<Discount> Discounts { get; }

        //scaffolded models
        IQueryable<Employees> Employees  { get; }
        IQueryable<EmployeeTerritories> EmployeeTerritories { get; }
        IQueryable<OrderDetails> OrderDetails { get; }
        IQueryable<Orders> Orders { get; }
        IQueryable<Region> Region { get; }
        IQueryable<Shippers> Shippers { get; }
        IQueryable<Territories> Territories { get; }

        void AddCustomer(Customer customer);
        void EditCustomer(Customer customer);

        void AddEmployee(Employees employee);
        void EditEmployee(Employees employee);

        CartItem AddToCart(CartItemJSON cartItemJSON);
    }
}
