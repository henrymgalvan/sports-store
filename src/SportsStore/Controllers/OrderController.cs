using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository _repo;
        private Cart _cart;

        public OrderController(IOrderRepository repo, Cart cartService)
        {
            _cart = cartService;
            _repo = repo;

        }


        [Authorize]
        public ViewResult List() => View(_repo.Orders.Where(p => !p.IsShipped));

        [Authorize]
        [HttpPost]
        public IActionResult SetAsShipped(int orderId)
        {
            var order = _repo.Orders.FirstOrDefault(p => p.OrderId == orderId);
            if (order != null)
            {
                order.IsShipped = true;
                _repo.SaveOrder(order);
            }

            return RedirectToAction(nameof(List));
        }

        public ViewResult Checkout() => View(new Order());

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (_cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty.");
            }

            if (ModelState.IsValid)
            {
                order.Lines = _cart.Lines.ToArray();
                _repo.SaveOrder(order);
                return RedirectToAction(nameof(Completed));
            }
            else
                return View(order);
        }

        public ViewResult Completed()
        {
            _cart.Clear();
            return View();
        }
    }
}
