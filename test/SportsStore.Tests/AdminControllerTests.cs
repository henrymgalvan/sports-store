using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SportsStore.Tests
{
    public class AdminControllerTests
    {
        [Fact]
        public void Index_Contains_All_Products()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(
                new Product[]
                {
                    new Product{ProductID=1, Name=It.IsAny<string>()},
                    new Product{ProductID=2, Name=It.IsAny<string>()},
                    new Product{ProductID=3, Name=It.IsAny<string>()},
                });

            var controller = new AdminController(mock.Object);

            var result = GetViewModel<IEnumerable<Product>>(controller.Index())?.ToArray();

            Assert.Equal(3, result.Length);
            Assert.Equal(1, result[0].ProductID);
            Assert.Equal(3, result[2].ProductID);
        }

        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }

        [Fact]
        public void Can_Edit_Product()
        {
            var mock = new Mock<IProductRepository>();

            mock.Setup(p => p.Products).Returns(
               new Product[]
               {
                   new Product{ProductID=1, Name="1"},
                   new Product{ProductID=2, Name="2"},
                   new Product{ProductID=3,Name="3"},
               }
                );

            var target = new AdminController(mock.Object);

            var result1 = GetViewModel<Product>(target.Edit(1));
            var result2 = GetViewModel<Product>(target.Edit(2));
            var result3 = GetViewModel<Product>(target.Edit(3));

            Assert.Equal("1", result1.Name);
            Assert.Equal("2", result2.Name);
            Assert.Equal("3", result3.Name);
        }

        [Fact]
        public void Cannot_Edit_Nonexistent_Product()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(p => p.Products).Returns(new Product[] { new Product { ProductID = 1 } });

            var target = new AdminController(mock.Object);

            var result = GetViewModel<Product>(target.Edit(2));

            Assert.Null(result);
        }

        [Fact]
        public void Can_Save_Valid_Changes()
        {
            var mock = new Mock<IProductRepository>();
            var tempData = new Mock<ITempDataDictionary>();
            var controller = new AdminController(mock.Object) { TempData = tempData.Object };

            var product = new Product { Name = "Test" };

            var result = controller.Edit(product);
            mock.Verify(p => p.SaveProduct(product));

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", (result as RedirectToActionResult).ActionName);
        }

        [Fact]
        public void Cannot_Save_Invalid_Chanegs()
        {
            var mock = new Mock<IProductRepository>();
            var target = new AdminController(mock.Object);

            var product = new Product { Name = "Test" };

            target.ModelState.AddModelError("error", "error");


            var result = target.Edit(product);

            mock.Verify(p => p.SaveProduct(It.IsAny<Product>()), Times.Never);
            Assert.IsType<ViewResult>(result);
        }
    }
}
