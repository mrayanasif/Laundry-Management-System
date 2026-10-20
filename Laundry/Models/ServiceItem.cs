using System.ComponentModel.DataAnnotations;

namespace Laundry.Models
{
    public class ServiceItem
    {
        [Key]
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
    }
}
