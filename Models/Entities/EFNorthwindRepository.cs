using System.Linq;

namespace Northwind.Models
{
    public class EFNorthwindRepository : INorthwindRepository
    {
        // the repository class depends on the NorthwindContext service
        // which was registered at application startup
        private NWFinal_19_CJLContext context;
        public EFNorthwindRepository(NWFinal_19_CJLContext ctx)
        {
            context = ctx;
        }
        // create IQueryables
        public IQueryable<Categories> Categories => context.Categories;
        public IQueryable<Customers> Customers => context.Customers;
        public IQueryable<Discounts> Discounts => context.Discounts;
        public IQueryable<Products> Products => context.Products;
        public IQueryable<Employees> Employees => context.Employees;
        public IQueryable<EmployeeTerritories> EmployeeTerritories => context.EmployeeTerritories;
        public IQueryable<OrderDetails> OrderDetails => context.OrderDetails;
        public IQueryable<Orders> Orders => context.Orders;
        public IQueryable<Region> Region => context.Region;
        public IQueryable<Shippers> Shippers => context.Shippers;
        public IQueryable<Suppliers> Suppliers => context.Suppliers;
        public IQueryable<Territories> Territories => context.Territories;

        public void AddCustomer(Customers customer)
        {
            context.Customers.Add(customer);
            context.SaveChanges();
        }

        public void EditCustomer(Customers customer)
        {
            var customerToUpdate = context.Customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId);
            customerToUpdate.Address = customer.Address;
            customerToUpdate.City = customer.City;
            customerToUpdate.Region = customer.Region;
            customerToUpdate.PostalCode = customer.PostalCode;
            customerToUpdate.Country = customer.Country;
            customerToUpdate.Phone = customer.Phone;
            customerToUpdate.Fax = customer.Fax;
            context.SaveChanges();
        }

        public void AddEmployee(Employees employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
        }

        public void EditEmployee(Employees employee)
        {
            var employeeToUpdate = context.Employees.FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);
            employeeToUpdate.LastName = employee.LastName;
            employeeToUpdate.FirstName = employee.FirstName;
            employeeToUpdate.Title = employee.Title;
            employeeToUpdate.TitleOfCourtesy = employee.TitleOfCourtesy;
            employeeToUpdate.BirthDate = employee.BirthDate;
            employeeToUpdate.HireDate = employee.HireDate;
            employeeToUpdate.Address = employee.Address;
            employeeToUpdate.City = employee.City;
            employeeToUpdate.Region = employee.Region;
            employeeToUpdate.PostalCode = employee.PostalCode;
            employeeToUpdate.Country = employee.Country;
            employeeToUpdate.HomePhone = employee.HomePhone;
            employeeToUpdate.Extension = employee.Extension;
            employeeToUpdate.ReportsTo = employee.ReportsTo;
            context.SaveChanges();
        }

        public CartItem AddToCart(CartItemJSON cartItemJSON)
        {
            int CustomerId = context.Customers.FirstOrDefault(c => c.Email == cartItemJSON.email).CustomerId;
            int ProductId = cartItemJSON.id;
            // check for duplicate cart item
            CartItem cartItem = context.CartItems.FirstOrDefault(ci => ci.ProductId == ProductId && ci.CustomerId == CustomerId);
            if (cartItem == null)
            {
                // this is a new cart item
                cartItem = new CartItem()
                {
                    CustomerId = CustomerId,
                    ProductId = cartItemJSON.id,
                    Quantity = cartItemJSON.qty
                };
                context.Add(cartItem);
            }
            else
            {
                // for duplicate cart item, simply update the quantity
                cartItem.Quantity += cartItemJSON.qty;
            }

            context.SaveChanges();
            cartItem.Product = context.Products.Find(cartItem.ProductId);
            return cartItem;
        }
    }
}
