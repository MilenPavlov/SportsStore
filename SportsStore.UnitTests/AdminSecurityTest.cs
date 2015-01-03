using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    /// <summary>
    /// Summary description for AdminSecurityTest
    /// </summary>
    [TestClass]
    public class AdminSecurityTest
    {
        [TestMethod]
        public void CanLoginWithValidCredentials()
        {
            var mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Athenticate("admin", "secret")).Returns(true);

            var model = new LoginViewModel
            {
                UserName = "admin",
                Password = "secret"
            };

            var target = new AccountController(mock.Object);

            var result = target.Login(model, "/MyUrl");

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyUrl", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void CannotLoginWithInvalidCredentials()
        {
            var mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Athenticate("badUser", "badPass")).Returns(false);

            var model = new LoginViewModel
            {
                UserName = "badUser",
                Password = "badPass"
            };

            var target = new AccountController(mock.Object);

            var result = target.Login(model, "/MyUrl");

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
