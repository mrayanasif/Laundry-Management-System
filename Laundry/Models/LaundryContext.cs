using Microsoft.EntityFrameworkCore;

namespace Laundry.Models
{
    public class LaundryContext : DbContext
    {
        public LaundryContext(DbContextOptions<LaundryContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ServiceItem> ServiceItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Staff> Staffs { get; set; }
    }
}