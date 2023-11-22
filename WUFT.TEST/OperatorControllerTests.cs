using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharedCodeLibrary.Email;
using SharedCodeLibrary.Email.RazorTemplate;
using SharedCodeLibrary.Email.RazorTemplate.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WUFT.MODEL;
using WUFT.NET.Controllers;
using WUFT.NET.ViewModels.Operator;
using WUFT.NET.ViewModels.Shared;

namespace WUFT.TEST
{
    [TestClass]
    public class OperatorControllerTests : ControllerBaseTest
    {
        
        private readonly OperatorController operatorController;
        private readonly OperatorController noAccessController;

        public OperatorControllerTests()
        {
            operatorController = CreateControllerForOperator();
        }

        [TestMethod]
        public void Index_Should_Return_View()
        {
            ActionResult result = operatorController.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Index_Should_Return_Expected_Count_of_Requests()
        {
            ViewResult result = operatorController.Index() as ViewResult;
            if(result != null)
            {
                Assert.IsInstanceOfType(result.Model, typeof(RequestIndexParentViewModel));
                RequestIndexParentViewModel model = result.Model as RequestIndexParentViewModel;
                if(model != null)
                {
                    Assert.AreEqual(model.Requests.Count, 3);
                }
            }
        }

        [TestMethod]
        public void View_Request_Should_Return_View()
        {
            ActionResult result = operatorController.ViewRequest(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Valid_ID_View_Request_Should_Have_Correct_Unit_Count()
        {
            ViewResult result = operatorController.ViewRequest(1) as ViewResult;
            if(result != null)
            {
                Assert.IsInstanceOfType(result.Model, typeof(ViewRequestViewModel));
                ViewRequestViewModel vm = result.Model as ViewRequestViewModel;
                if(vm != null)
                {
                    Assert.IsInstanceOfType(vm.FlaggedUnits, typeof(List<FlaggedUnitLineItem>));
                    Assert.AreEqual(2, vm.FlaggedUnits.Count);
                    
                }
            }
        }

        [TestMethod]
        public void Invalid_ID_View_Request_Should_Return_Error_Message()
        {
            ViewResult result = operatorController.ViewRequest(-1) as ViewResult;
            if(result != null)
            {
                Assert.IsInstanceOfType(result.Model, typeof(ViewRequestViewModel));
                ViewRequestViewModel vm = result.Model as ViewRequestViewModel;
                if(vm != null)
                {
                    Assert.AreEqual(true, vm.InvalidID);
                }
            }
        }

        public OperatorController CreateControllerForOperator()
        {
            var identity = new GenericIdentity("AMR\\ALFISCHE");
            var principal = new GenericPrincipal(identity, new string[] { "ISE Logistics_WUFT_Operator" });

            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User).Returns(principal);

            var operatorController = new OperatorController(stub, new EmailTemplateFactory(new DefaultEmailTemplateSettings()), new Emailer());
            operatorController.ControllerContext = mock.Object;

            return operatorController;
        }
    }
}
