using System;
using System.Linq;
using System.Text;

using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    /// <summary>
    /// Summary description for ImageTests
    /// </summary>
    [TestClass]
    public class ImageTests
    {    
        [TestMethod]
        public void CanRetrieveImageData()
        {
            var prod = new Product
            {
                ProductID = 2,
                Name = "Test",
                ImageData = new byte[]{},
                ImageMimeType = "image/png"
            };

            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID = 1, Name = "P1"},
                prod,
                new Product{ProductID = 3, Name = "P3"}
            }.AsQueryable());

            var target = new ProductsController(mock.Object);

            var result = target.GetImage(2);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(prod.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void CannotRetrieveImageDataForInvalidId()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product{ProductID = 1, Name = "P1"},           
                new Product{ProductID = 3, Name = "P3"}
            }.AsQueryable());

            var target = new ProductsController(mock.Object);

            var result = target.GetImage(100);

            Assert.IsNull(result);
        }
    }
}
