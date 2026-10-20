using Microsoft.AspNetCore.Mvc;
using Laundry.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Laundry.Controllers
{
    public class AdminController : Controller
    {
        private readonly LaundryContext _context;

        public object SmsService { get; private set; }

        public AdminController(LaundryContext context)
        {
            _context = context;
        }
        // Agar banda Login nahi hai, toh yeh usay andar nahi aane dega.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = HttpContext.Session.GetString("UserID");
            var userRole = HttpContext.Session.GetString("UserRole"); 

            if (string.IsNullOrEmpty(userId) || userRole != "Admin")
            {
       
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }

            base.OnActionExecuting(context);
        }
        //==================================DashBoard====================================

                                       // Stats + Recent Orders

        public async Task<IActionResult> Index()
        {
  
                // ORDER COUNTS
            // Total Orders (Cancelled ko nikal kar)
            var totalOrders = await _context.Orders.CountAsync(o => o.Status != "Cancelled");
            ViewBag.TotalOrders = totalOrders;

            // Completed Orders
            var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed" || o.Status == "Delivered");
            ViewBag.CompletedOrders = completedOrders;

            // Pending Orders
            var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
            ViewBag.PendingOrders = pendingOrders;

            // Users Count
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            // 2. FINANCIAL STATS
            var incomeReceived = _context.Orders
                .Where(o => o.Status == "Completed" || o.Status == "Delivered")
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;
            ViewBag.IncomeReceived = incomeReceived;

            // PENDING PAYMENT (Sirf Pending/Processing orders ka paisa)
            var incomePending = _context.Orders
                .Where(o => o.Status == "Pending" || o.Status == "Processing" || o.Status == "Out For Delivery")
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;
            ViewBag.IncomePending = incomePending;


            // 3. GRAPH DATA
            var data = await _context.Orders
                .Where(o => o.OrderDate >= DateTime.Now.AddDays(-7) && o.Status != "Cancelled")
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(o => o.TotalAmount) })
                .ToListAsync();

            ViewBag.ChartDates = data.Select(d => d.Date.ToString("dd MMM")).ToArray();
            ViewBag.ChartValues = data.Select(d => d.Total).ToArray();

            //MONTHLY GOAL LOGIC 

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var monthlyRevenue = await _context.Orders
                .Where(o => o.OrderDate >= startOfMonth && (o.Status == "Completed" || o.Status == "Delivered"))
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            decimal targetGoal = 100000;
            int achievementPct = 0;
            if (targetGoal > 0)
            {
                achievementPct = (int)((monthlyRevenue / targetGoal) * 100);
            }        
            ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.MonthlyTarget = targetGoal;
            ViewBag.TargetPercentage = Math.Min(achievementPct, 100);          
            var orders = await _context.Orders
                                    .Include(o => o.User)
                                    .OrderByDescending(o => o.OrderId)
                                    .Take(10)
                                    .ToListAsync();
            return View(orders);
        }

        //UPDATE ORDER STATUS 
        [HttpPost]
        public IActionResult UpdateOrder(int id, string status)
        {
            var order = _context.Orders.Find(id);

            if (order != null)
            {
                if (string.IsNullOrEmpty(status))
                {
                    order.Status = "Pending";
                }
                else
                {
                    order.Status = status;
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
      
                                            //REPORTS SECTION
        public IActionResult Reports()
        {
            //Daily Collection (Aaj kal ki kamaai)
            ViewBag.DailyCollection = _context.Orders
                .Where(o => o.OrderDate.Date == DateTime.Today)
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            //Due Payments (Jo Pending hain)
            var dueOrders = _context.Orders
                .Include(o => o.User)
                .Where(o => o.Status == "Pending")
                .ToList();

            // Garment-Wise Collection (Konsa kapra sab se zyada dhula or konsa sab sai zyada revenue generate kar rha hai)
            var garmentStats = _context.OrderDetails
                .GroupBy(od => od.ItemName)
                .Select(g => new
                {
                    ItemName = g.Key,
                    Count = g.Sum(od => od.Quantity),
                    Income = g.Sum(od => od.Price * od.Quantity)
                })
                .OrderByDescending(x => x.Income)
                .ToList();

            // 4. Customer Total Business (Kis customer ne kitna paisa diya)
            var customerBusiness = _context.Orders
                .Include(o => o.User)
                .GroupBy(o => o.User.FullName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalSpent = g.Sum(o => o.TotalAmount),
                    OrdersCount = g.Count()
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(10) 
                .ToList();
          
            ViewBag.DueOrders = dueOrders;
            ViewBag.GarmentStats = garmentStats;
            ViewBag.CustomerStats = customerBusiness;

            return View();
        }

                                            //  CASH BOOK 
     
        public IActionResult CashBook()
        {
            // Saaray Orders nikalo (User ka data sath mein)
            var allOrders = _context.Orders.Include(o => o.User).ToList();
            var receivedOrders = allOrders
                .Where(o => o.Status == "Completed" || o.Status == "Delivered")
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            // Cancelled ko hum hisaab se nikal dete hain orders 
            var pendingOrders = allOrders
                .Where(o => o.Status != "Completed" && o.Status != "Delivered" && o.Status != "Cancelled")
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            // Totals bhi bhej dete hain
            ViewBag.TotalCashIn = receivedOrders.Sum(o => o.TotalAmount);
            ViewBag.TotalPending = pendingOrders.Sum(o => o.TotalAmount);

            // Lists ViewBag mein daal di
            ViewBag.ReceivedOrders = receivedOrders;
            ViewBag.PendingOrders = pendingOrders;

            return View();
        }

                                // MANAGE ITEMS (Items List)     
        public async Task<IActionResult> ManageItems()
        {
            var items = await _context.ServiceItems.ToListAsync();
            return View(items);
        }

        // Create Item (GET ka method)
        // 1. GET: Add Item ka Form dikhao
        public IActionResult CreateItem()
        {
            return View();
        }

        // 2. POST: Jab Admin "Save" dabaye
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateItem(ServiceItem item)
        {
            if (ModelState.IsValid)
            {
                // Database mein naya item add karo
                _context.ServiceItems.Add(item);
                _context.SaveChanges();            
                return RedirectToAction("ManageItems");
            }
            return View(item);
        }

        // ---------------- EDIT ITEM (POST) ----------------
        [HttpPost]
        public IActionResult UpdateItem(ServiceItem model)
        {
            var item = _context.ServiceItems.Find(model.ItemId);

            if (item != null)
            {
               
                item.ItemName = model.ItemName;
                item.Price = model.Price;

                _context.SaveChanges(); 
            }

            return RedirectToAction("ManageItems"); 
        }

        // ---------------- DELETE ITEM  ----------------
        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.ServiceItems.Find(id);

            if (item != null)
            {
                _context.ServiceItems.Remove(item); 
                _context.SaveChanges();
            }

            return RedirectToAction("ManageItems");
        }



                                         //(MANAGE USERS)
     
        [HttpPost]
        public IActionResult UpdateUser(User model)
        {
            var user = _context.Users.Find(model.UserId);

            if (user != null)
            {             
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.Contact = model.Contact;
                user.Address = model.Address;

                _context.SaveChanges(); 
            }

            
            return RedirectToAction("Users"); 
        }
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Users");
        }

        // GET: Admin/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)          
                .Include(o => o.OrderDetails)   
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null) return NotFound();

            return View(order);
        }  
        public IActionResult InvoiceUsers()
        {
            var users = _context.Orders
                            .Include(o => o.User)
                            .Select(o => o.User)
                            .Distinct()
                            .ToList();

            return View(users);
        }    
        public IActionResult UserInvoices(int id) 
        {
            var userOrders = _context.Orders
                                     .Where(o => o.UserId == id) 
                                     .OrderByDescending(o => o.OrderDate) 
                                     .ToList();

            var user = _context.Users.Find(id);
            ViewBag.UserName = user != null ? user.FullName : "Customer";

            return View(userOrders);
        }
        

                                            // Chat Page 
        public IActionResult Chat()
        {
           
            var chatList = _context.Users
                .Where(u => u.Role != "Admin")
                .Where(u => _context.Orders.Any(o => o.UserId == u.UserId))
                .Select(u => new UserChat
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Contact = u.Contact,

                  
                    LatestOrderId = _context.Orders
                                    .Where(o => o.UserId == u.UserId)
                                    .OrderByDescending(o => o.OrderId)
                                    .Select(o => o.OrderId)
                                    .FirstOrDefault(),
                    LatestStatus = _context.Orders
                                   .Where(o => o.UserId == u.UserId)
                                   .OrderByDescending(o => o.OrderId)
                                   .Select(o => o.Status)
                                   .FirstOrDefault() ?? "New"
                })
                .ToList(); 
    
            foreach (var chat in chatList)
            {
                if (chat.LatestOrderId > 0)
                {
                    var items = _context.OrderDetails
                        .Where(d => d.OrderId == chat.LatestOrderId)
                        .Select(d => d.Quantity + "x " + d.ItemName) 
                        .ToList();
                    chat.ItemSummary = string.Join(", ", items);
                }
                else
                {
                    chat.ItemSummary = ""; 
                }
            }

            return View(chatList);
        }

                                     // --- STAFF MANAGEMENT START ---
        public IActionResult Staff()
        {
            var staffList = _context.Staffs.ToList(); // Database se saara staff uthao
            return View(staffList);
        }

        //(Create)
        [HttpPost]
        public IActionResult AddStaff(Staff model)
        {
            model.Status = "Active";
            _context.Staffs.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Staff");
        }

        //(Delete)
        public IActionResult DeleteStaff(int id)
        {
            var staff = _context.Staffs.Find(id);
            if (staff != null)
            {
                _context.Staffs.Remove(staff);
                _context.SaveChanges();
            }
            return RedirectToAction("Staff");
        }

        //(Edit)
        [HttpGet]
        public IActionResult EditStaff(int id)
        {
            var staff = _context.Staffs.Find(id);
            if (staff == null) return RedirectToAction("Staff");
            return View(staff);
        }

        // 5. Edit Staff (Save karne ke liye)
        [HttpPost]
        public IActionResult EditStaff(Staff model)
        {
            _context.Staffs.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Staff");
        }

                                     // --- SALARY SLIP  ---
        [HttpGet]
        public IActionResult SalarySlip(int id, int deductions)
        {
            var staff = _context.Staffs.Find(id);

            if (staff == null) return NotFound();

            // Data View ko bhejo
            ViewBag.Deductions = deductions;
            ViewBag.NetSalary = staff.BasicSalary - deductions;
            ViewBag.CurrentDate = DateTime.Now.ToString("dd MMMM yyyy");

            return View(staff);
        }
        // --- STAFF MANAGEMENT END ---


    }
}