using System;
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations;

namespace Laundry.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; } // Customer ID
        public virtual User User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Pending, Ready
        public virtual List<OrderDetail> OrderDetails { get; set; }
    }
}