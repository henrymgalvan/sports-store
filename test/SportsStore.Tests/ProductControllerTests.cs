using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using SportsStore.Components;
using SportsStore.Controllers;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SportsStore.Tests
{
    public class ProductControllerTests
    {
        [Fact]
        public void Can_Paginate()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
                new Product {ProductID = 2, Name = "P2"},
                new Product {ProductID = 3, Name = "P3"},
                new Product {ProductID = 4, Name = "P4"},
                new Product {ProductID = 5, Name = "P5"} });

            var controller = new ProductController(mock.Object) { PageSize = 3 };

            var result = controller.List(null, 2).ViewData.Model as ProductsListViewModel;

            Product[] array = result.Products.ToArray();

            Assert.True(array.Length == 2);
            Assert.Equal(array[0].Name, "P4");
            Assert.Equal(array[1].Name, "P5");
        }

        [Fact]
        public void RepositoryAcces_Once()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[] { It.IsAny<Product>() });

            var controller = new ProductController(mock.Object);

            controller.List(null);

            mock.VerifyGet(m => m.Products, Times.Exactly(2));
        }

        [Fact]
        public void Can_Filter_Products()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
                new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
                new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
                new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
                new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            });

            var controller = new ProductController(mock.Object);

            var result = (controller.List("Cat2", 1).ViewData.Model as ProductsListViewModel).Products.ToArray();

            Assert.Equal(2, result.Length);
            Assert.True(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.True(result[1].Name == "P4" && result[0].Category == "Cat2");
        }

        [Fact]
        public void Can_Select_Categories_NavigationView()
        {
            var mock = new Mock<IProductRepository>();

            mock.Setup(p => p.Products).Returns(new Product[] {
                new Product{ Name= "1", ProductID = 1,Category = "C1"},
                new Product{ Name="1", ProductID = 2,Category = "C2"},
                new Product{ Name="3", ProductID = 3,Category = "C3"},
                new Product{ Name= "4", ProductID = 4,Category = "C4"},

            });

            var controller = new NavigationMenuViewComponent(mock.Object);

            string[] result = ((IEnumerable<string>)((controller.Invoke() as ViewViewComponentResult).ViewData.Model)).ToArray();


            Assert.True(Enumerable.SequenceEqual(new string[] { "C1", "C2", "C3", "C4" }, result));
        }


        [Fact]
        public void Can_Add_Item_Cart()
        {
            var p1 = new Product { Name = "P1", ProductID = 1 };
            var p2 = new Product { Name = "P2", ProductID = 2 };
            var p3 = new Product { Name = "P3", ProductID = 3 };

            var cart = new Cart();

            cart.AddItem(p1, 1);
            cart.AddItem(p2, 1);
            cart.AddItem(p3, 1);

            CartLine[] cartLine = cart.Lines.ToArray();

            Assert.Equal(3, cartLine.Length);
        }

        [Fact]
        public void Can_Increment_Quantity_Cart()
        {
            var p1 = new Product { Name = "P1", ProductID = 1 };


            var cart = new Cart();

            cart.AddItem(p1, 1);
            cart.AddItem(p1, 5);

            var result = cart.Lines.ToArray();

            Assert.Equal(6, result[0].Quantity);
        }

        [Fact]
        public void Can_Remove_Items_Cart()
        {
            var p1 = new Product { Name = "P1", ProductID = 1 };
            var p2 = new Product { Name = "P2", ProductID = 2 };
            var p3 = new Product { Name = "P3", ProductID = 3 };


            var cart = new Cart();

            cart.AddItem(p1, 1);
            cart.AddItem(p2, 2);
            cart.AddItem(p3, 1);
            cart.AddItem(p1, 100);


            cart.RemoveLine(p1);

            var result = cart.Lines;

            Assert.Equal(0, result.Count(p => p.Product.ProductID == 1));
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void Can_Calculate_TotalPrices_Cart()
        {

            var p1 = new Product { Name = "P1", ProductID = 1, Price = 1 };
            var p2 = new Product { Name = "P2", ProductID = 2, Price = 1.5M };
            var p3 = new Product { Name = "P3", ProductID = 3, Price = 2.5M };

            var cart = new Cart();

            cart.AddItem(p1, 1);
            cart.AddItem(p2, 2);
            cart.AddItem(p3, 1);

            var totalPrice = cart.ComputeValue();

            Assert.Equal(6.5M, totalPrice);
        }

        [Fact]
        public void Can_Reset_Cart()
        {
            var p1 = new Product { Name = "P1", ProductID = 1, Price = 1 };
            var p2 = new Product { Name = "P2", ProductID = 2, Price = 1.5M };
            var p3 = new Product { Name = "P3", ProductID = 3, Price = 2.5M };

            var cart = new Cart();

            cart.AddItem(p1, 1);
            cart.AddItem(p2, 10);
            cart.AddItem(p3, 10);

            cart.Clear();

            Assert.Equal(0, cart.Lines.Count());
        }


    }
}
