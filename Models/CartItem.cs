using System.ComponentModel.DataAnnotations;

namespace Northwind.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int Quantity { get; set; }

        public Customers Customer { get; set; }
        public Products Product { get; set; }
    }
}
