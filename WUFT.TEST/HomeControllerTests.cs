using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUFT.NET.Controllers;
using System.Security.Principal;
using System.Threading;
using WUFT.DATA.Fakes;
using Moq;

namespace WUFT.TEST
{
    [TestClass]
    public class HomeControllerTests : ControllerBaseTest
    {
        [TestMethod]
        public void HomeIndex_QRE_User_Should_Redirect_to_QRE_Index()
        {
            //Arrange
            var controller = CreateControllerForQRE();

            //Act
            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;

            //Assert
            Assert.AreEqual("QRE", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void HomeIndex_Operator_User_Should_Redirect_to_Operator_Index()
        {
            var controller = CreateControllerForOperator();

            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;

            Assert.AreEqual("Operator", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void HomeIndex_No_Access_User_Should_Redirect_to_AccessDenied()
        {
            var controller = CreateControllerForNoAccess();

            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;

            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("AccessDenied", result.RouteValues["action"]);
        }

        public HomeController CreateControllerForQRE()
        {
            var identity = new GenericIdentity("AMR\\ALFISCHE");
            var principal = new GenericPrincipal(identity, new string[] { "ISE Logistics_WUFT_QRE" });

            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User).Returns(principal);

            var homeController = new HomeController(stub);
            homeController.ControllerContext = mock.Object;

            return homeController;
        }

        public HomeController CreateControllerForOperator()
        {
            var identity = new GenericIdentity("AMR\\ALFISCHE");
            var principal = new GenericPrincipal(identity, new string[] { "ISE Logistics_WUFT_Operator" });

            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User).Returns(principal);

            var homeController = new HomeController(stub);
            homeController.ControllerContext = mock.Object;

            return homeController;
        }

        public HomeController CreateControllerForNoAccess()
        {
            var identity = new GenericIdentity("AMR\\ALFISCHE");
            var principal = new GenericPrincipal(identity, new string[] { "" });

            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User).Returns(principal);

            var homeController = new HomeController(stub);
            homeController.ControllerContext = mock.Object;

            return homeController;
        }
    }
}
