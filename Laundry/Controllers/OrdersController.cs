using Laundry.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Laundry.Controllers
{
    public class OrdersController : Controller
    {
        private readonly LaundryContext _context;

        public OrdersController(LaundryContext context)
        {
            _context = context;
        }


        public IActionResult Create()
        {
            // Database se saaray rate list items utha kai website per display karo
            var items = _context.ServiceItems.ToList();
            ViewBag.ServiceItems = items;
            return View();
        }


        [HttpPost]
        public IActionResult Create(List<int> ItemIds, List<int> Quantities, string InvoiceType)
        {

            var userIdStr = HttpContext.Session.GetString("UserID");
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);
            decimal grandTotal = 0;

            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = 0
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();


            for (int i = 0; i < ItemIds.Count; i++)
            {
                if (Quantities[i] > 0)
                {
                    var item = _context.ServiceItems.Find(ItemIds[i]);
                    if (item != null)
                    {
                        var detail = new OrderDetail
                        {
                            OrderId = newOrder.OrderId,
                            ItemName = item.ItemName,
                            Quantity = Quantities[i],
                            Price = item.Price
                        };
                        grandTotal += (item.Price * Quantities[i]);
                        _context.OrderDetails.Add(detail);
                    }
                }
            }


            newOrder.TotalAmount = grandTotal;
            _context.SaveChanges();

            return RedirectToAction("MyOrders");
        }

        public IActionResult MyOrders()
        {
            // 1. Session Check: Pata lagao kaun login hai
            var userIdStr = HttpContext.Session.GetString("UserID");

            // Agar login nahi hai, toh Login page par bhejo
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            // String ID ko number mein convert karo
            int userId = int.Parse(userIdStr);

            
            var myOrders = _context.Orders
                                   .Where(o => o.UserId == userId) 
                                   .OrderByDescending(o => o.OrderDate) 
                                   .ToList();

            return View(myOrders);
        }
        // ---------------- INVOICE DELETE KA Code ----------------

        // 1. GET: Confirmation Delete ka Page Dikhao
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);

            // Agar order nahi mila ya status Pending nahi hai toh wapis bhej do
            if (order == null || order.Status != "Pending")
            {
                return RedirectToAction("MyOrders");
            }

            return View(order);
        }

        // 2. POST: Jab user "Confirm Delete" dabaye to yeh delete hojai ga or data base sai bhi haata dai ga
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var order = _context.Orders.Find(id);

            if (order != null && order.Status == "Pending")
            {
                
                var details = _context.OrderDetails.Where(od => od.OrderId == id).ToList();
                _context.OrderDetails.RemoveRange(details);

               
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }

            return RedirectToAction("MyOrders");
        }


        // ---------------- INVOICE EDIT KA Code ----------------
        public IActionResult Edit(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null || order.Status != "Pending")
            {
                return RedirectToAction("MyOrders");
            }

            var allItems = _context.ServiceItems.ToList();
            var existingDetails = _context.OrderDetails
                                          .Where(od => od.OrderId == id)
                                          .ToList();
            ViewBag.DetailsCount = existingDetails.Count;

            ViewBag.ServiceItems = allItems;
            ViewBag.ExistingDetails = existingDetails;
            ViewBag.OrderId = id;

            return View();
        }

        [HttpPost]
        public IActionResult Edit(int id, List<int> ItemIds, List<int> Quantities)
        {
            
            var oldDetails = _context.OrderDetails.Where(od => od.OrderId == id).ToList();
            _context.OrderDetails.RemoveRange(oldDetails);

            decimal grandTotal = 0;

           
            for (int i = 0; i < ItemIds.Count; i++)
            {
                if (Quantities[i] > 0)
                {
                    var item = _context.ServiceItems.Find(ItemIds[i]);

                    if (item != null)
                    {
                        var detail = new OrderDetail
                        {
                            OrderId = id,

                            
                            ItemId = item.ItemId,
                         

                            ItemName = item.ItemName,
                            Quantity = Quantities[i],
                            Price = item.Price
                        };

                        grandTotal += (item.Price * Quantities[i]);
                        _context.OrderDetails.Add(detail);
                    }
                }
            }

          
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.TotalAmount = grandTotal;
                _context.SaveChanges();

            }

            return RedirectToAction("MyOrders");
        }

    }
}
