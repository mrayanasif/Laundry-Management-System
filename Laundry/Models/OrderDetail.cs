using System.ComponentModel.DataAnnotations;

namespace Laundry.Models
{
    public class OrderDetail
    {
        [Key]
        public int DetailId { get; set; }  

        public int OrderId { get; set; }        
        public int ItemId { get; set; }

        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual Order Order { get; set; }
    }
}