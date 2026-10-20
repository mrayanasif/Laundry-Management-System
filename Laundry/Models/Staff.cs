using System;
using System.ComponentModel.DataAnnotations;

namespace Laundry.Models
{
    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        public string Name { get; set; }
        public string Designation { get; set; } // Rider, Washer, Manager
        public double BasicSalary { get; set; }
        public string Status { get; set; } = "Active"; // Active, Resigned
    }
}