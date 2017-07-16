using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private IProductRepository _repo;


        public NavigationMenuViewComponent(IProductRepository repo)
        {
            _repo = repo;
        }

        // called when used in Razor view
        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(_repo.Products.Select(x => x.Category).Distinct().OrderBy(x => x));
        }
    }
}
