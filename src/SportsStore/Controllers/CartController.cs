using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SportsStore.Infrastructure;
using SportsStore.Models.ViewModels;
using System.Diagnostics;

namespace SportsStore.Controllers
{
    public class CartController : Controller
    {
        private IProductRepository _repo;
        private Cart _cart;

        public CartController(IProductRepository repo, Cart cartService)
        {
            _repo = repo;
            _cart = cartService;
        }

        public ViewResult Index(string returnUrl)
        {
            return View(new CartIndexViewModel() { Cart = _cart, ReturnUrl = returnUrl });
        }

        public RedirectResult AddToCart(int productId, string returnUrl)
        {

            Debug.WriteLine("****AddToCartCalled****");

            var product = _repo.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
            {
                _cart.AddItem(product, 1);
            }

            //return RedirectToAction("Index", new { returnUrl });
            return Redirect(returnUrl);
        }

        public RedirectToActionResult RemoveFromCart(int productId, string returnUrl)
        {
            var product = _repo.Products.FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
            {
                _cart.RemoveLine(product);
            }
            return RedirectToAction("Index", new { returnUrl });
        }
    }
}
