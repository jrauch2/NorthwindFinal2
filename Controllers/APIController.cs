using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Northwind.Models;

namespace Northwind.Controllers
{
    public class APIController : Controller
    {
        // this controller depends on the NorthwindRepository
        private INorthwindRepository repository;
        public APIController(INorthwindRepository repo) => repository = repo;

        [HttpGet, Route("api/product")]
        // returns all products
        public IEnumerable<Products> Get() => repository.Products.Where(p => !p.Discontinued).OrderBy(p => p.ProductName);

        [HttpGet, Route("api/product/{id}")]
        // returns specific product
        public Products Get(int id) => repository.Products.FirstOrDefault(p => p.ProductId == id);

        [HttpGet, Route("api/product/discontinued")]
        // returns all products where discontinued = true
        public IEnumerable<Products> GetDiscontinued() => repository.Products.Where(p => p.Discontinued).OrderBy(p => p.ProductName);


        [HttpGet, Route("api/product/reorder")]
        // returns all products where inventory level is below or equal to reorder level
        public IEnumerable<Products> GetReorder()
        {
            return repository.Products
                .Where(p => !p.Discontinued && p.UnitsInStock + p.UnitsOnOrder <= p.ReorderLevel)
                .OrderBy(p => p.ProductName);
        }

        [HttpGet, Route("api/product/outofstock")]
        // returns all products in a specific category where discontinued = true/false
        public IEnumerable<Products> GetOutOfStock() => repository.Products
            .Where(p => p.UnitsInStock + p.UnitsOnOrder == 0).OrderBy(p => p.ProductName);

        [HttpGet, Route("api/category/{CategoryId}/product")]
        // returns all products in a specific category
        public IEnumerable<Products> GetByCategory(int CategoryId) =>
            repository.Products
            .Where(p => p.CategoryId == CategoryId && !p.Discontinued)
            .OrderBy(p => p.ProductName);

        [HttpGet, Route("api/category/{CategoryId}/product/discontinued")]
        // returns all products in a specific category where discontinued = true
        public IEnumerable<Products> GetByCategoryDiscontinued(int CategoryId) =>
            repository.Products
            .Where(p => p.CategoryId == CategoryId && p.Discontinued)
            .OrderBy(p => p.ProductName);

        [HttpPost, Route("api/addtocart")]
        // adds a row to the cartitem table
        public CartItem Post([FromBody] CartItemJSON cartItem) => repository.AddToCart(cartItem);

        [HttpGet, Route("api/category/{CategoryId}/product/reorder")]
        // returns all products in a specific category where inventory level is below or equal to reorder level
        public IEnumerable<Products> GetByCategoryReorder(int CategoryId, bool reorder)
        {
            return repository.Products
                .Where(p => p.CategoryId == CategoryId &&
                    !p.Discontinued &&
                    p.UnitsInStock + p.UnitsOnOrder <= p.ReorderLevel)
                .OrderBy(p => p.ProductName);
        }

        [HttpGet, Route("api/category/{CategoryId}/product/outofstock")]
        // returns all products in a specific category where discontinued = true/false
        public IEnumerable<Products> GetByCategoryDiscontinued(int CategoryId, bool discontinued) =>
            repository.Products
            .Where(p => p.CategoryId == CategoryId && p.UnitsInStock + p.UnitsOnOrder == 0)
            .OrderBy(p => p.ProductName);
    }
}
