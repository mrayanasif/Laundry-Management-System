namespace Laundry.Models
{
    public class UserChat
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Contact { get; set; }
        public int LatestOrderId { get; set; } 
        public string LatestStatus { get; set; } 
        public string ItemSummary { get; set; }
    }
}