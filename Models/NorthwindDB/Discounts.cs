using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Models
{
    public partial class Discounts
    {
        public int DiscountId { get; set; }
        public int? Code { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartTime { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndTime { get; set; }
        public int? ProductId { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public virtual Products Product { get; set; }
    }
}
