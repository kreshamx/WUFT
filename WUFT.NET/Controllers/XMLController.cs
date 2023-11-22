using SharedCodeLibrary.Email;
using SharedCodeLibrary.Email.RazorTemplate.Factories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using WUFT.DATA;
using WUFT.EF;
using WUFT.MODEL;
using WUFT.NET.filters;
using WUFT.NET.Util;
using WUFT.NET.ViewModels.Service;

namespace WUFT.NET.Controllers
{
    public class XMLController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly IEmailTemplateFactory etf;
        private readonly IEmailer emailer;

        public XMLController(IUnitOfWork uow, IEmailTemplateFactory etf, IEmailer emailer)
        {
            this.uow = uow;
            this.etf = etf;
            this.emailer = emailer;
        }

        public enum ResponseStatus { SUCCESS, FAILURE }

        public const string SUCCESS = "SUCCESS";
        public const string FAILURE = "FAILURE";
        private LotQtyRequestModel modelQty;

        // GET: XML
        public ActionResult Index()
        {
            return View();
        }

        [RestAPIAttribute]
        public ActionResult EndDemixWlotRequest()
        {
            EndDemixWlotRequestModel model = new EndDemixWlotRequestModel();
            string request = ViewBag.XMLBody;
            if (!String.IsNullOrEmpty(request))
            {
                model = ParseEndRequest(request);
                var flagRequestID = 0;
                var result = UpdateECDUnitsForStoppedRequest(model, out flagRequestID);
                if (flagRequestID > 0)
                {
                    var result2 = UpdateFlagRequest(flagRequestID, model.StartTime, model.EndTime);
                    if (result2)
                    {
                        SendEmails(flagRequestID);
                        return Content(" Box and Flag Request have been updated in system.", "text/xml");
                    }
                    else
                        return Content("Could not update Flag Request. Please contact support.", "text/xml");
                }
                else
                {
                    return Content(result, "text/xml");
                }
            }

            return Content("Error making request. Pleast contact support.", "text/xml");
        }

        private string UpdateECDUnitsForStoppedRequest(EndDemixWlotRequestModel model, out int flagRequestID)
        {
            var dispoID = model.Mode.Equals("Demix", StringComparison.OrdinalIgnoreCase) ? 8 : 7;
            var req = new List<ECD_WarehouseDemix>();
            flagRequestID = 0;

            if (model.Mode.Equals("Demix", StringComparison.OrdinalIgnoreCase))
            {
                req = uow.ECD_WarehouseDemixes.GetAll().IncludeMultiple(d => d.FlagRequest, d => d.FlagRequest.Warehouse)
                .Where(w => (
                w.OriginalLotNumber == model.LotNumber
                && w.OriginalBoxNumber == model.BoxNumber
                && w.FlagRequest.MRBID == model.ReferenceNumber
                && w.FlagRequest.Warehouse.SiteCode == model.Facility
                && w.FlagRequest.DispositionID == 8
                && !w.UnitFound
                )).ToList();
            }
            else
            {
                req = uow.ECD_WarehouseDemixes.GetAll().IncludeMultiple(x => x.FlagRequest, x => x.FlagRequest.Warehouse)
                .Where(w => (
                    w.OriginalLotNumber == model.LotNumber
                    && w.OriginalBoxNumber == model.BoxNumber
                    && w.DestinationLotNumber == model.UnmergeLotNumber
                    && w.FlagRequest.MRBID == model.ReferenceNumber
                    && w.FlagRequest.Warehouse.SiteCode == model.Facility
                    && w.FlagRequest.DispositionID == 7 // WLOT 
                    && !w.UnitFound
                    )).ToList();
            }

            if (req.Count == 0)
                return " Could not find Request.";

            req.ForEach(x =>
            {
                if (model.FoundUnits.Contains(x.SubstrateVisualID))
                    x.UnitFoundButRequestStopped = true;
                uow.ECD_WarehouseDemixes.Update(x);
            });

            //Update box
            var tempRequestID = flagRequestID = req.Count > 0 ? req.FirstOrDefault().FlagRequestID : 0;
            if (tempRequestID > 0)
            {
                BoxRequestStatus _affectedBox = uow.BoxRequestStatuses.GetAll().FirstOrDefault(x =>
                    x.BoxNumber == model.BoxNumber &&
                    x.FlagRequestID == tempRequestID
                );
                var _completedWithMissingID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName.Equals("Stopped with Units Missing")).RequestStatusID;
                _affectedBox.RequestStatusID = _completedWithMissingID;
                _affectedBox.CompletedOn = DateTime.UtcNow;
                _affectedBox.LastModifiedOn = DateTime.UtcNow;
                _affectedBox.StartTime = model.StartTime;
                _affectedBox.EndTime = model.EndTime;
                uow.BoxRequestStatuses.Update(_affectedBox);
            }

            try
            {
                uow.SaveChanges();
                return "";
            }
            catch (Exception e)
            {
                return "Error updating box and unit information. Please contact support.";
            }
        }

        [RestAPIAttribute]
        public ActionResult DemixRequest()
        {

            string xmlString = "";
            string request = ViewBag.XMLBody;
            LotRequestModel model = new LotRequestModel();
            LotQtyRequestModel modelQty = new LotQtyRequestModel();
            LotResponseModel response = new LotResponseModel();
            Update_originalQuantity_flaggedUnit response2 = new Update_originalQuantity_flaggedUnit();

            //Change made by rshanm3x for FS-INC100003465
            modelQty = ParseQuantityDataRequest(request);

            Update_originalQuantity_flaggedUnit(modelQty);

            var inProcess = uow.RequestStatuses.GetAll().First(x => x.RequestStatusName.Equals("In Process")).RequestStatusID;

            if (!String.IsNullOrEmpty(request))
            {
                model = ParseUnitContainmentDataRequest(request);

                var demixreq = uow.ECD_WarehouseDemixes.GetAll().IncludeMultiple(x => x.FlagRequest, x => x.FlagRequest.Warehouse)
                .Where(w => (
                    w.OriginalLotNumber == model.LotNumber
                    && w.OriginalBoxNumber == model.BoxNumber
                    && w.FlagRequest.MRBID == model.ReferenceNumber
                    && w.FlagRequest.Warehouse.SiteCode == model.Facility
                    && w.FlagRequest.DispositionID == 8 // Demix 
                    && !w.UnitFound
                    && w.FlagRequest.RequestStatusID == inProcess
                    )).ToList();

                if (demixreq.Count > 0)
                {
                    response.Lots.Add(new LotResponseModel.Lot()
                    {
                        LotNumber = demixreq[0].OriginalLotNumber,
                        ReferenceNumber = demixreq[0].FlagRequest.MRBID,
                        Facility = demixreq[0].FlagRequest.WarehouseName,
                        Quantity = demixreq[0].OriginalQty,
                        Boxes = new List<LotResponseModel.Box>()
                    });

                    List<LotResponseModel.Unit> unitlist = demixreq.Select(s => new LotResponseModel.Unit() { VisualId = s.SubstrateVisualID, Status = string.Empty }).ToList();

                    response.Lots[0].Boxes.Add(new LotResponseModel.Box()
                    {
                        BoxNumber = demixreq[0].OriginalBoxNumber,
                        Units = unitlist
                    });
                }
            }
            xmlString = SerializeToXml(response);
            return Content(xmlString, "text/xml");
        }

        //Created by rshanm3x for FS-INC100003465
        private void Update_originalQuantity_flaggedUnit(LotQtyRequestModel modelQty)
        {
            LotQtyRequestModel q = new LotQtyRequestModel();
            SqlConnection conn = new SqlConnection();
            string strOriginalQty = null;
            string strOriginalBoxNum = null;
            string strLotNum = null;

            q = modelQty;

            strOriginalBoxNum = q.BoxNumber;
            strOriginalQty = q.Quantity;
            strLotNum = q.LotNumber;
           
                          
            //var update = uow.ECD_WarehouseDemixes.GetAll().IncludeMultiple(x => x.FlagRequest)
            //        .FirstOrDefault(w =>
            //            w.FlagRequest.MRBID == q.ReferenceNumber &&
            //            w.OriginalLotNumber == q.LotNumber &&
            //            w.OriginalBoxNumber == q.BoxNumber);

            //if (update != null)
            //{
            //    update.OriginalQty = q.Quantity;
            //    update.FlaggedUnitsQty = 1;
            //}
            try
            {

                //SP call made by rshanm3x to update Original Quantity on initial input by User during scanning process.
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["WUFTDbContext"].ConnectionString;
                SqlCommand sql = new SqlCommand("updateOriginalQuantity", conn);
                sql.CommandType = System.Data.CommandType.StoredProcedure;
                sql.CommandTimeout = 60;

                SqlParameter sBoxNum = new SqlParameter("@originalBoxNum", SqlDbType.NVarChar);
                sBoxNum.Value = strOriginalBoxNum;
                sql.Parameters.Add(sBoxNum);

                SqlParameter sLotNum = new SqlParameter("@lotNum", SqlDbType.NVarChar);
                sLotNum.Value = strLotNum;
                sql.Parameters.Add(sLotNum);

                SqlParameter sOriginalQty = new SqlParameter("@originalQty", SqlDbType.NVarChar);
                sOriginalQty.Value = strOriginalQty;
                sql.Parameters.Add(sOriginalQty);

                conn.Open();
                var reader = sql.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed writing units");
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        //Created by rshanm3x for FS-INC100003465
        private LotQtyRequestModel ParseQuantityDataRequest(string XMLQtyInput)
        {
            LotQtyRequestModel modelQ = new LotQtyRequestModel();

            using (XmlReader reader = XmlReader.Create(new StringReader(XMLQtyInput)))
            {
                reader.ReadToFollowing("Lot");
                modelQ.LotNumber = reader.GetAttribute("LotNumber");
                modelQ.UnmergeLotNumber = reader.GetAttribute("UnmergeLotNumber");
                modelQ.ReferenceNumber = reader.GetAttribute("ReferenceNumber");
                modelQ.Facility = reader.GetAttribute("Facility");
                modelQ.Quantity = reader.GetAttribute("Quantity");
                reader.ReadToFollowing("BoxNumber");
                modelQ.BoxNumber = reader.ReadElementContentAsString();
            }
            return modelQ;
        }

        [RestAPIAttribute]
        public ActionResult WlotRequest()
        {
            LotRequestModel model = new LotRequestModel();

            string xmlString = "";
            string request = ViewBag.XMLBody;
            LotResponseModel response = new LotResponseModel();

            if (!String.IsNullOrEmpty(request))
            {
                model = ParseUnitContainmentDataRequest(request);

                var inProcess = uow.RequestStatuses.GetAll().First(x => x.RequestStatusName.Equals("In Process")).RequestStatusID;

                var demixreq = uow.ECD_WarehouseDemixes.GetAll().IncludeMultiple(x => x.FlagRequest, x => x.FlagRequest.Warehouse)
                    .Where(w => (
                        w.OriginalLotNumber == model.LotNumber
                        && w.OriginalBoxNumber == model.BoxNumber
                        && w.DestinationLotNumber == model.UnmergeLotNumber
                        && w.FlagRequest.MRBID == model.ReferenceNumber
                        && w.FlagRequest.Warehouse.SiteCode == model.Facility
                        && w.FlagRequest.DispositionID == 7 // WLOT 
                        && !w.UnitFound
                        && w.FlagRequest.RequestStatusID == inProcess
                        )).ToList();


                if (demixreq.Count > 0)
                {
                    response.Lots.Add(new LotResponseModel.Lot()
                    {
                        LotNumber = demixreq[0].OriginalLotNumber,
                        UnmergeLotNumber = demixreq[0].DestinationLotNumber,
                        ReferenceNumber = demixreq[0].FlagRequest.MRBID,
                        Facility = demixreq[0].FlagRequest.WarehouseName,
                        Quantity = demixreq[0].OriginalQty,
                        Boxes = new List<LotResponseModel.Box>()
                    });

                    List<LotResponseModel.Unit> unitlist = demixreq.Select(s => new LotResponseModel.Unit() { VisualId = s.SubstrateVisualID, Status = string.Empty }).ToList();

                    response.Lots[0].Boxes.Add(new LotResponseModel.Box()
                    {
                        BoxNumber = demixreq[0].OriginalBoxNumber,
                        Units = unitlist
                    });
                }
            }
            xmlString = SerializeToXml(response);
            return Content(xmlString, "text/xml");
        }

        [HttpGet]
        public ActionResult WUFTConnectionTest()
        {
            return Content("true", "text/xml");
        }

        [HttpPost]
        [RestAPIAttribute]
        public ActionResult ConnectionTest()
        {

            string request = ViewBag.XMLBody;
            ConnectionTestViewModel model = new ConnectionTestViewModel();
            string xmlString = "";
            string scname = "";

            if (!String.IsNullOrEmpty(request))
            {
                model = (ConnectionTestViewModel)DeserializeXmlToClass(request, model);

                var scontroller = uow.StationControllers.GetAll().Where(w => w.ComputerName == model.ComputerName).FirstOrDefault();

                if (scontroller != null)
                {
                    scontroller.LastLogin = DateTime.UtcNow;
                    scontroller.AppVersion = model.AppVersion;
                    scontroller.StationControllerSite = model.StationControllerSite;
                    scontroller.UserDomain = model.UserDomain;
                    scontroller.UserName = model.UserName;
                    scontroller.WindowsVersion = model.WindowsVersion;

                    uow.SaveChanges();

                    var sc = uow.StationControllers.GetAll().Where(w => w.ComputerName == model.ComputerName).FirstOrDefault();
                    scname = sc.StationControllerName;
                    //sc.StationControllerName = scname;
                    //uow.SaveChanges();
                }
                else
                {
                    uow.StationControllers.Add(new StationController
                    {
                        LastLogin = DateTime.UtcNow,
                        AppVersion = model.AppVersion,
                        ComputerName = model.ComputerName,
                        StationControllerName = model.StationControllerName,
                        StationControllerSite = model.StationControllerSite,
                        UserName = model.UserName,
                        UserDomain = model.UserDomain,
                        WindowsVersion = model.WindowsVersion
                    });
                    uow.SaveChanges();

                    var sc = uow.StationControllers.GetAll().Where(w => w.ComputerName == model.ComputerName).FirstOrDefault();
                    scname = "SC" + sc.StationControllerID.ToString();
                    sc.StationControllerName = scname;
                    uow.SaveChanges();
                }

                ConnectionResponseViewModel srvm = new ConnectionResponseViewModel();
                srvm.Status = SUCCESS;
                srvm.StationControllerName = scname;
                xmlString = SerializeToXml(srvm);
            }

            return Content(xmlString, "text/xml");
        }

        [RestAPIAttribute]
        public ActionResult SubmitToEMSQueue()
        {
            UpdateStatusViewModel model = new UpdateStatusViewModel();

            string xmlString = "";
            string request = ViewBag.XMLBody;
            UpdateResponseViewModel response = new UpdateResponseViewModel();


            int flagRequestId = 0;

            if (!String.IsNullOrEmpty(request))
            {
                model = ParseUpdateStatusRequest(request);

                //Update ECD units using xml request information
                UpdateECDUnits(model,out flagRequestId);

                //If Flag ID is still 0, there was a problem
                response.Status = flagRequestId == 0 ? FAILURE : SUCCESS;

                //Update the affected FlagRequest
                if (flagRequestId != 0)
                {
                    if (UpdateFlagRequest(flagRequestId, model.StartTime, model.EndTime))
                    {
                        response.Status = SUCCESS;
                        SendEmails(flagRequestId);
                    }
                    else
                    {
                        response.Status = FAILURE;
                    }
                }
            }
            else
            {
                response.Status = FAILURE;
            }

            xmlString = SerializeToXml(response);
            return Content(xmlString, "text/xml");
        }

        private void SendEmails(int flagRequestId)
        {
            var completedID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName == "Completed").RequestStatusID;
            var missingUnitsID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName == "Completed with Units Missing").RequestStatusID;

            EmailController eController = new EmailController(uow, etf, emailer, User);

            var _requestStatus = uow.FlagRequests.GetById(flagRequestId).RequestStatusID;
            if (_requestStatus == completedID)
                eController.SendCompletedEmail(flagRequestId);
            else if (_requestStatus == missingUnitsID)
                eController.SendCompletedWithMissingUnitsEmail(flagRequestId);
        }

        private bool UpdateFlagRequest(int flagRequestId, DateTime startTime, DateTime endTime)
        {
            var _request = uow.FlagRequests.GetAll().IncludeMultiple(x => x.RequestStatus).FirstOrDefault(x => x.FlagRequestID == flagRequestId);
            var _boxes = uow.BoxRequestStatuses.GetAll().Where(x => x.FlagRequestID == flagRequestId).IncludeMultiple(x => x.RequestStatus).ToList();

            //No new requests left
            if (_boxes.All(x => x.RequestStatus.RequestStatusName != "New Request") && _boxes.All(x => x.RequestStatus.RequestStatusName != "In Process"))
            {
                if (_boxes.All(x => x.RequestStatus.RequestStatusName == "Completed"))
                {
                    _request.RequestStatus = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName.Equals("Completed"));
                }
                else
                {
                    _request.RequestStatus = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName.Equals("Stopped with Units Missing"));
                }
                _request.CompletedOn = DateTime.UtcNow;
                _request.CompletedBy = (User as ISEPrincipal).WUFTUser.IdSid;
                _request.StartTime = startTime;
                _request.EndTime = endTime;
            }

            //CYA
            _request.LastModified = DateTime.UtcNow;
            _request.LastModifiedBy = (User as ISEPrincipal).WUFTUser.IdSid;

            uow.FlagRequests.Update(_request);

            try
            {
                uow.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void UpdateECDUnits(UpdateStatusViewModel model, out int flagRequestId)
        {
            int tempInt;
            DateTime tempDate;
            flagRequestId = 0;
            int _tempRequestID = -1;

            UpdateStatusViewModel.Lot lot = model.Lots.FirstOrDefault();
            var _badBoxes = uow.BoxRequestStatuses.GetAll().Select(x => x.BoxNumber).ToList();
            UpdateStatusViewModel.Box goodBox = lot.Boxes.FirstOrDefault(x => _badBoxes.Contains(x.BoxNumber));
            UpdateStatusViewModel.Box badBox = lot.Boxes.FirstOrDefault(x => !_badBoxes.Contains(x.BoxNumber));

            foreach (UpdateStatusViewModel.Unit unit in badBox.Units)
            {
                var row = uow.ECD_WarehouseDemixes.GetAll().IncludeMultiple(x => x.FlagRequest)
                    .FirstOrDefault(w =>
                        w.FlagRequest.MRBID == lot.ReferenceNumber &&
                        w.OriginalLotNumber == lot.LotNumber &&
                        w.SubstrateVisualID == unit.VisualId);

                if (row != null)
                {
                    _tempRequestID = flagRequestId = row.FlagRequestID;

                    // Lot Updates
                    row.SCID = lot.SCID;
                    row.MaterialMasterNumber = lot.MaterialMasterNumber;

                    // Box Updates
                    row.DestinationBoxNumber = badBox.BoxNumber;
                    row.StartDateTime = DateTime.TryParse(badBox.StartDateTime, out tempDate) ? DateTime.Parse(badBox.StartDateTime) : DateTime.MinValue;
                    row.EndDateTime = DateTime.TryParse(badBox.EndDateTime, out tempDate) ? DateTime.Parse(badBox.EndDateTime) : DateTime.MinValue;
                    row.LabelQuantity = Int32.TryParse(badBox.LabelQuantity, out tempInt) ? Int32.Parse(badBox.LabelQuantity) : 0;

                    // Unit Updates
                    row.SequenceNumber = Int32.TryParse(unit.SequenceNumber, out tempInt) ? Int32.Parse(unit.SequenceNumber) : 0;
                    row.LoadStatus = unit.LoadStatus;
                    row.CarrierX = Int32.TryParse(unit.CarrierX, out tempInt) ? Int32.Parse(unit.CarrierX) : 0;
                    row.CarrierY = Int32.TryParse(unit.CarrierY, out tempInt) ? Int32.Parse(unit.CarrierY) : 0;
                    row.ScanDateTime = DateTime.TryParse(unit.ScanDateTime, out tempDate) ? DateTime.Parse(unit.ScanDateTime) : DateTime.MinValue;
                    row.CarrierID = unit.CarrierId;
                    row.DispositionID = Int32.TryParse(unit.Status, out tempInt) ? Int32.Parse(unit.Status) : 0;
                    row.UnitFound = true;
                    row.LastUpdateBy = (User as ISEPrincipal).WUFTUser.IdSid;
                    row.LastUpdateOn = DateTime.UtcNow;

                    uow.ECD_WarehouseDemixes.Update(row);
                }
                try
                {
                    uow.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed writing units");
                    Console.WriteLine(e.Message);
                }
            }

            BoxRequestStatus _affectedBox = uow.BoxRequestStatuses.GetAll().FirstOrDefault(x =>
                x.BoxNumber == goodBox.BoxNumber &&
                x.FlagRequestID == _tempRequestID
            );

            var _completeID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName.Equals("Completed")).RequestStatusID;
            _affectedBox.RequestStatusID = _completeID;
            _affectedBox.CompletedOn = DateTime.UtcNow;
            _affectedBox.LastModifiedOn = DateTime.UtcNow;
            _affectedBox.StartTime = model.StartTime;
            _affectedBox.EndTime = model.EndTime;
            _affectedBox.GoodBoxUnitQty = Int32.Parse(goodBox.LabelQuantity);
            uow.BoxRequestStatuses.Update(_affectedBox);

            try
            {
                uow.SaveChanges();
                EmailController eController = new EmailController(uow, etf, emailer, User);
                eController.SendCompletedBoxEmail(_affectedBox.FlagRequestID, _affectedBox.BoxNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        EndDemixWlotRequestModel ParseEndRequest(string XMLInput)
        {
            EndDemixWlotRequestModel model = new EndDemixWlotRequestModel();
            DateTime s;
            DateTime e;
            using (XmlReader reader = XmlReader.Create(new StringReader(XMLInput)))
            {
                reader.ReadToFollowing("EndRequest");
                model.LotNumber = reader.GetAttribute("LotNumber");
                var unmerge = reader.GetAttribute("UnmergeLotNumber");
                model.UnmergeLotNumber = unmerge == null ? string.Empty : unmerge;
                model.ReferenceNumber = reader.GetAttribute("MRB");
                model.Facility = reader.GetAttribute("Facility");
                model.Mode = reader.GetAttribute("Mode");
                DateTime.TryParse(reader.GetAttribute("StartTime"), out s);
                model.StartTime = s;
                DateTime.TryParse(reader.GetAttribute("EndTime"), out e);
                model.EndTime = e;
                model.BoxNumber = reader.GetAttribute("BoxNumber");
                if (reader.ReadToDescendant("FoundUnits"))//get first unit
                {
                    if (reader.Read())
                        model.FoundUnits.Add(reader.Value);
                }

                while (reader.ReadToFollowing("FoundUnits"))//get the rest
                {
                    if (reader.Read())
                        model.FoundUnits.Add(reader.Value);
                }
            }
            return model;
        }

        LotRequestModel ParseUnitContainmentDataRequest(string XMLInput)
        {
            LotRequestModel model = new LotRequestModel();

            using (XmlReader reader = XmlReader.Create(new StringReader(XMLInput)))
            {
                reader.ReadToFollowing("Lot");
                model.LotNumber = reader.GetAttribute("LotNumber");
                model.UnmergeLotNumber = reader.GetAttribute("UnmergeLotNumber");
                model.ReferenceNumber = reader.GetAttribute("ReferenceNumber");
                model.Facility = reader.GetAttribute("Facility");
                model.Quantity = reader.GetAttribute("Quantity");
                reader.ReadToFollowing("BoxNumber");
                model.BoxNumber = reader.ReadElementContentAsString();

            }
            return model;
        }

        UpdateStatusViewModel ParseUpdateStatusRequest(string XMLInput)
        {
            UpdateStatusViewModel usmxl = new UpdateStatusViewModel();
            UpdateStatusViewModel response = (UpdateStatusViewModel)DeserializeXmlToClass(XMLInput, usmxl);

            return response;
        }

        public string SerializeToXml(object input)
        {
            XmlSerializer ser = new XmlSerializer(input.GetType());

            string result = string.Empty;

            //Removes the XML Name Spaces
            XmlSerializerNamespaces xns = new XmlSerializerNamespaces();
            xns.Add(string.Empty, string.Empty);

            using (MemoryStream memStm = new MemoryStream())
            {
                ser.Serialize(memStm, input, xns);

                memStm.Position = 0;
                result = new StreamReader(memStm).ReadToEnd();
            }

            return result;
        }

        public static object DeserializeXmlToClass(string xmlString, object classType)
        {
            using (TextReader textReader = new StringReader(xmlString))
            {
                var deserializer = new XmlSerializer(classType.GetType());
                var convertedXml = deserializer.Deserialize(textReader);
                textReader.Close();
                return convertedXml;
            }
        }
    }


}