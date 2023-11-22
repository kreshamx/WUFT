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
using WUFT.NET.Controllers;
using WUFT.NET.ViewModels.QRE;
using WUFT.NET.ViewModels.Shared;

namespace WUFT.TEST
{
    [TestClass]
    public class QREControllerTests : ControllerBaseTest
    {
        
        private readonly QREController qreController;
        private readonly QREController noAccessController;

        public QREControllerTests()
        {
            qreController = CreateControllerForQRE();
            noAccessController = CreateControllerForNoAccess();
            //serializedUnits = "[{\"WarehouseDemixID\":0,\"OriginalLotNumber\":\"N304346A\",\"OriginalBoxNumber\":\"RVR16703\",\"EventID\":0,\"FacilityId\":0,\"SubstrateVisualID\":\"3E309209A0972\",\"DispositionID\":0,\"StatusID\":1,\"DestinationLotNumber\":null,\"DestinationBoxNumber\":null,\"SCID\":null,\"MaterialMasterNumber\":null,\"StartDateTime\":\"\\/Date(1430950919342)\\/\",\"EndDateTime\":\"\\/Date(1430950919343)\\/\",\"LabelQuantity\":0,\"SequenceNumber\":0,\"LoadStatus\":null,\"CarrierX\":0,\"CarrierY\":0,\"ScanDateTime\":0,\"ScanType\":null,\"CarrierID\":null,\"CreatedOn\":\"\\/Date(1430950919340)\\/\",\"CreatedBy\":null,\"LastUpdateOn\":\"\\/Date(1430950919342)\\/\",\"LastUpdateBy\":null,\"FlagRequest\":null,\"FlagRequestID\":0,\"Warehouse\":null,\"WarehouseName\":\"TX03\",\"Status\":null},{\"WarehouseDemixID\":0,\"OriginalLotNumber\":\"N304346A\",\"OriginalBoxNumber\":\"RVR16703\",\"EventID\":0,\"FacilityId\":0,\"SubstrateVisualID\":\"3E309209A2467\",\"DispositionID\":0,\"StatusID\":1,\"DestinationLotNumber\":null,\"DestinationBoxNumber\":null,\"SCID\":null,\"MaterialMasterNumber\":null,\"StartDateTime\":\"\\/Date(1430950919510)\\/\",\"EndDateTime\":\"\\/Date(1430950919510)\\/\",\"LabelQuantity\":0,\"SequenceNumber\":0,\"LoadStatus\":null,\"CarrierX\":0,\"CarrierY\":0,\"ScanDateTime\":0,\"ScanType\":null,\"CarrierID\":null,\"CreatedOn\":\"\\/Date(1430950919510)\\/\",\"CreatedBy\":null,\"LastUpdateOn\":\"\\/Date(1430950919510)\\/\",\"LastUpdateBy\":null,\"FlagRequest\":null,\"FlagRequestID\":0,\"Warehouse\":null,\"WarehouseName\":\"TX03\",\"Status\":null},{\"WarehouseDemixID\":0,\"OriginalLotNumber\":\"N304346A\",\"OriginalBoxNumber\":\"RVR16703\",\"EventID\":0,\"FacilityId\":0,\"SubstrateVisualID\":\"3E309209A2480\",\"DispositionID\":0,\"StatusID\":1,\"DestinationLotNumber\":null,\"DestinationBoxNumber\":null,\"SCID\":null,\"MaterialMasterNumber\":null,\"StartDateTime\":\"\\/Date(1430950919657)\\/\",\"EndDateTime\":\"\\/Date(1430950919657)\\/\",\"LabelQuantity\":0,\"SequenceNumber\":0,\"LoadStatus\":null,\"CarrierX\":0,\"CarrierY\":0,\"ScanDateTime\":0,\"ScanType\":null,\"CarrierID\":null,\"CreatedOn\":\"\\/Date(1430950919657)\\/\",\"CreatedBy\":null,\"LastUpdateOn\":\"\\/Date(1430950919657)\\/\",\"LastUpdateBy\":null,\"FlagRequest\":null,\"FlagRequestID\":0,\"Warehouse\":null,\"WarehouseName\":\"TX03\",\"Status\":null},{\"WarehouseDemixID\":0,\"OriginalLotNumber\":\"N304346A\",\"OriginalBoxNumber\":\"RVR16704\",\"EventID\":0,\"FacilityId\":0,\"SubstrateVisualID\":\"3E309209A2438\",\"DispositionID\":0,\"StatusID\":1,\"DestinationLotNumber\":null,\"DestinationBoxNumber\":null,\"SCID\":null,\"MaterialMasterNumber\":null,\"StartDateTime\":\"\\/Date(1430950919800)\\/\",\"EndDateTime\":\"\\/Date(1430950919800)\\/\",\"LabelQuantity\":0,\"SequenceNumber\":0,\"LoadStatus\":null,\"CarrierX\":0,\"CarrierY\":0,\"ScanDateTime\":0,\"ScanType\":null,\"CarrierID\":null,\"CreatedOn\":\"\\/Date(1430950919800)\\/\",\"CreatedBy\":null,\"LastUpdateOn\":\"\\/Date(1430950919800)\\/\",\"LastUpdateBy\":null,\"FlagRequest\":null,\"FlagRequestID\":0,\"Warehouse\":null,\"WarehouseName\":\"CNA8\",\"Status\":null},{\"WarehouseDemixID\":0,\"OriginalLotNumber\":\"N304346A\",\"OriginalBoxNumber\":\"RVR16704\",\"EventID\":0,\"FacilityId\":0,\"SubstrateVisualID\":\"3E309209A2444\",\"DispositionID\":0,\"StatusID\":1,\"DestinationLotNumber\":null,\"DestinationBoxNumber\":null,\"SCID\":null,\"MaterialMasterNumber\":null,\"StartDateTime\":\"\\/Date(1430950919938)\\/\",\"EndDateTime\":\"\\/Date(1430950919938)\\/\",\"LabelQuantity\":0,\"SequenceNumber\":0,\"LoadStatus\":null,\"CarrierX\":0,\"CarrierY\":0,\"ScanDateTime\":0,\"ScanType\":null,\"CarrierID\":null,\"CreatedOn\":\"\\/Date(1430950919938)\\/\",\"CreatedBy\":null,\"LastUpdateOn\":\"\\/Date(1430950919938)\\/\",\"LastUpdateBy\":null,\"FlagRequest\":null,\"FlagRequestID\":0,\"Warehouse\":null,\"WarehouseName\":\"CNA8\",\"Status\":null},{\"WarehouseDemixID\":0,\"OriginalLotNumber\":\"N304346A\",\"OriginalBoxNumber\":\"RVR16704\",\"EventID\":0,\"FacilityId\":0,\"SubstrateVisualID\":\"3E309209A0500\",\"DispositionID\":0,\"StatusID\":1,\"DestinationLotNumber\":null,\"DestinationBoxNumber\":null,\"SCID\":null,\"MaterialMasterNumber\":null,\"StartDateTime\":\"\\/Date(1430950920089)\\/\",\"EndDateTime\":\"\\/Date(1430950920089)\\/\",\"LabelQuantity\":0,\"SequenceNumber\":0,\"LoadStatus\":null,\"CarrierX\":0,\"CarrierY\":0,\"ScanDateTime\":0,\"ScanType\":null,\"CarrierID\":null,\"CreatedOn\":\"\\/Date(1430950920089)\\/\",\"CreatedBy\":null,\"LastUpdateOn\":\"\\/Date(1430950920089)\\/\",\"LastUpdateBy\":null,\"FlagRequest\":null,\"FlagRequestID\":0,\"Warehouse\":null,\"WarehouseName\":\"CNA8\",\"Status\":null},{\"WarehouseDemixID\":0,\"OriginalLotNumber\":\"N3043760\",\"OriginalBoxNumber\":\"RVR16555\",\"EventID\":0,\"FacilityId\":0,\"SubstrateVisualID\":\"3E309209A0473\",\"DispositionID\":0,\"StatusID\":3,\"DestinationLotNumber\":null,\"DestinationBoxNumber\":null,\"SCID\":null,\"MaterialMasterNumber\":null,\"StartDateTime\":\"\\/Date(1430950920288)\\/\",\"EndDateTime\":\"\\/Date(1430950920288)\\/\",\"LabelQuantity\":0,\"SequenceNumber\":0,\"LoadStatus\":null,\"CarrierX\":0,\"CarrierY\":0,\"ScanDateTime\":0,\"ScanType\":null,\"CarrierID\":null,\"CreatedOn\":\"\\/Date(1430950920288)\\/\",\"CreatedBy\":null,\"LastUpdateOn\":\"\\/Date(1430950920288)\\/\",\"LastUpdateBy\":null,\"FlagRequest\":null,\"FlagRequestID\":0,\"Warehouse\":null,\"WarehouseName\":\"TX03\",\"Status\":null},{\"WarehouseDemixID\":0,\"OriginalLotNumber\":\"N3043760\",\"OriginalBoxNumber\":\"RVR16555\",\"EventID\":0,\"FacilityId\":0,\"SubstrateVisualID\":\"3E309209A0527\",\"DispositionID\":0,\"StatusID\":3,\"DestinationLotNumber\":null,\"DestinationBoxNumber\":null,\"SCID\":null,\"MaterialMasterNumber\":null,\"StartDateTime\":\"\\/Date(1430950920438)\\/\",\"EndDateTime\":\"\\/Date(1430950920438)\\/\",\"LabelQuantity\":0,\"SequenceNumber\":0,\"LoadStatus\":null,\"CarrierX\":0,\"CarrierY\":0,\"ScanDateTime\":0,\"ScanType\":null,\"CarrierID\":null,\"CreatedOn\":\"\\/Date(1430950920438)\\/\",\"CreatedBy\":null,\"LastUpdateOn\":\"\\/Date(1430950920438)\\/\",\"LastUpdateBy\":null,\"FlagRequest\":null,\"FlagRequestID\":0,\"Warehouse\":null,\"WarehouseName\":\"TX03\",\"Status\":null},{\"WarehouseDemixID\":0,\"OriginalLotNumber\":\"N3044070\",\"OriginalBoxNumber\":\"RVR16666\",\"EventID\":0,\"FacilityId\":0,\"SubstrateVisualID\":\"3E309209A0533\",\"DispositionID\":0,\"StatusID\":3,\"DestinationLotNumber\":null,\"DestinationBoxNumber\":null,\"SCID\":null,\"MaterialMasterNumber\":null,\"StartDateTime\":\"\\/Date(1430950920582)\\/\",\"EndDateTime\":\"\\/Date(1430950920582)\\/\",\"LabelQuantity\":0,\"SequenceNumber\":0,\"LoadStatus\":null,\"CarrierX\":0,\"CarrierY\":0,\"ScanDateTime\":0,\"ScanType\":null,\"CarrierID\":null,\"CreatedOn\":\"\\/Date(1430950920582)\\/\",\"CreatedBy\":null,\"LastUpdateOn\":\"\\/Date(1430950920582)\\/\",\"LastUpdateBy\":null,\"FlagRequest\":null,\"FlagRequestID\":0,\"Warehouse\":null,\"WarehouseName\":\"CNA8\",\"Status\":null}]";
        }

        [TestMethod]
        public void No_QRE_Role_Should_Redirect_to_AccessDenied()
        {
            var controller = CreateControllerForNoAccess();
            RedirectToRouteResult result = controller.Index() as RedirectToRouteResult;

            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("AccessDenied", result.RouteValues["action"]);
        }

        [TestMethod]
        public void Create_Should_Create_Correct_Number_of_Requests()
        {
            ConfirmRequestViewModel vm = new ConfirmRequestViewModel{
                QRELoadJobID = "eestewar201505271933235295",
                MRBID = "1111111"
            };
            qreController.Create(vm);

            Assert.AreEqual(stub.FlagRequestsGet().GetAll().Count(), 9);
        }

        [TestMethod]
        public void Successful_Create_Should_Redirect_to_Index()
        {
            ConfirmRequestViewModel vm = new ConfirmRequestViewModel{
                QRELoadJobID = "eestewar201505271933235295",
                MRBID = "1111111"
            };
            RedirectToRouteResult result = qreController.Create(vm) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("QRE", result.RouteValues["controller"]);
        }

        //[TestMethod]
        //public void Error_Create_Should_Stay_On_Page()
        //{

        //}

        [TestMethod]
        public void Index_Should_Return_View()
        {
            ActionResult result = qreController.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Index_Should_Return_Expected_Count_of_Requests()
        {
            ViewResult result = qreController.Index() as ViewResult;
            if(result != null)
            {
                Assert.IsInstanceOfType(result.Model, typeof(RequestIndexParentViewModel));
                RequestIndexParentViewModel model = result.Model as RequestIndexParentViewModel;
                if (model != null)
                {
                    Assert.AreEqual(model.Requests.Count, 3);
                }
            }
        }

        public QREController CreateControllerForQRE()
        {
            var identity = new GenericIdentity("AMR\\ALFISCHE");
            var principal = new GenericPrincipal(identity, new string[] { "ISE Logistics_WUFT_QRE" });

            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User).Returns(principal);

            var qreController = new QREController(stub, new EmailTemplateFactory(new DefaultEmailTemplateSettings()), new Emailer());
            qreController.ControllerContext = mock.Object;

            return qreController;
        }

        public QREController CreateControllerForNoAccess()
        {
           
            var identity = new GenericIdentity("");
            var principal = new GenericPrincipal(identity, null);

            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User).Returns(principal);
            mock.Setup(p => p.HttpContext.User.IsInRole("ISE Logistics_WUFT_QRE")).Returns(false);

            var qreController = new QREController(stub, new EmailTemplateFactory(new DefaultEmailTemplateSettings()), new Emailer());
            qreController.ControllerContext = mock.Object;

            return qreController;
        }

    }
}
