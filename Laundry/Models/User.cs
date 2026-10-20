using System.ComponentModel.DataAnnotations;

namespace Laundry.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
  
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // Admin or Customer
        public string Address { get; set; }
        public string Contact { get; set; }
      
    }
}
