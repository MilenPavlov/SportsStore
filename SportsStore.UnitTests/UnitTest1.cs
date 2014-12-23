using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {             
                 new Product {ProductID = 1, Name = "P1"},
                 new Product {ProductID = 2, Name = "P2"},
                 new Product {ProductID = 3, Name = "P3"},
                 new Product {ProductID = 4, Name = "P4"},
                 new Product {ProductID = 5, Name = "P5"}
            });

            var controller = new ProductsController(mock.Object) {PageSize = 3};

            var result = (ProductsListViewModel)controller.List(null,2).Model;


            var prodArray = result.Products.ToArray();

            Assert.IsTrue(prodArray.Length ==2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            HtmlHelper myHelper = null;

            var pagingInfo = new PagingInfo
            {
                CurrentPage = 2, 
                TotalItems = 28,
                ItemsPerPage = 10
            };

            Func<int, string> pageUrlDelegate = i => "Page" + i;

            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // Assert
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
            + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
            result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            var mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {               
                 new Product {ProductID = 1, Name = "P1"},
                 new Product {ProductID = 2, Name = "P2"},
                 new Product {ProductID = 3, Name = "P3"},
                 new Product {ProductID = 4, Name = "P4"},
                 new Product {ProductID = 5, Name = "P5"}
            });

            var controller = new ProductsController(mock.Object) {PageSize = 3};

            var result = (ProductsListViewModel) controller.List(null, 2).Model;

            var pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
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

            var controller = new ProductsController(mock.Object);

            var result = ((ProductsListViewModel) controller.List("Cat2", 1).Model).Products.ToArray();


            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");

        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                 new Product {ProductID = 2, Name = "P2", Category = "Apples"},
                 new Product {ProductID = 3, Name = "P3", Category = "Plums"},
                 new Product {ProductID = 4, Name = "P4", Category = "Oranges"}
            });

            var target = new NavController(mock.Object);

            var results = ((IEnumerable<string>) target.Menu().Model).ToArray();

            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            var mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1", Category = "Apples"},
                new Product {ProductID = 4, Name = "P2", Category = "Oranges"},
            });

            var target = new NavController(mock.Object);

            string categoryToSelect = "Apples";

            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
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

            var target = new ProductsController(mock.Object) {PageSize = 3};

            int res1 = ((ProductsListViewModel) target.List("Cat1").Model).PagingInfo.TotalItems;

            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;

            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;

            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
