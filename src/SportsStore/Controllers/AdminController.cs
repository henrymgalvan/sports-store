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
    [Authorize]
    public class AdminController : Controller
    {
        private IProductRepository _repo;

        public AdminController(IProductRepository repo)
        {
            _repo = repo;
        }

        public ViewResult Index() => View(_repo.Products);

        public ViewResult Edit(int productId)
        {
            return View(_repo.Products.FirstOrDefault(p => p.ProductID.Equals(productId)));
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _repo.SaveProduct(product);
                TempData["message_green"] = $"{product.Name} has been saved";
                return RedirectToAction("Index");
            }
            else
                return View(product);
        }

        public ViewResult Create()
        {
            return View("Edit", new Product());
        }

        public IActionResult Delete(int productID)
        {
            var deletedProduct = _repo.DeleteProduct(productID);

            TempData["message_red"] = deletedProduct == null ? "Product not found" : $"{deletedProduct.Name} was deleted";

            return RedirectToAction("Index");
        }
    }
}
