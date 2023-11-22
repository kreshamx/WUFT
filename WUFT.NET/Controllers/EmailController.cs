using SharedCodeLibrary.Email;
using SharedCodeLibrary.Email.RazorTemplate.Factories;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using WUFT.DATA;
using WUFT.MODEL;
using WUFT.EF;
using WUFT.NET.Util;
using WUFT.NET.ViewModels.Emails;
using WUFT.NET.Views.Shared;
using System.Security.Principal;

namespace WUFT.NET.Controllers
{
    public class EmailController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly IEmailer emailer;
        private readonly IEmailTemplateFactory etf;
        private readonly IPrincipal user;
        private readonly PersonHelper _personHelper;

        public EmailController(IUnitOfWork uow, IEmailTemplateFactory etf, IEmailer emailer, IPrincipal User)
        {
            this.uow = uow;
            this.etf = etf;
            this.user = User;
            this.emailer = emailer;
            this._personHelper = new PersonHelper(uow);
        }

        public EmailController(IUnitOfWork uow, IEmailTemplateFactory etf, IEmailer emailer)
        {
            this.uow = uow;
            this.etf = etf;
            this.emailer = emailer;
            this._personHelper = new PersonHelper(uow);
        }

        public void SendCreateEmail(FlagRequest x)
        {           
            var emailTemplate = new FlagRequestCreatedEmailViewModel
            {
                Disposition = uow.Dispositions.GetById(x.DispositionID).DispositionName,
                UnmergeDispositionName = uow.Dispositions.GetById(7).DispositionName,
                ScrapDispositionName = uow.Dispositions.GetById(8).DispositionName,
                ReleaseDispositionName = uow.Dispositions.GetById(9).DispositionName,
                FlaggedUnitCount = x.FlaggedUnits.Count.ToString(),
                LotCount = x.FlaggedUnits.GroupBy(g => g.OriginalLotNumber).Count().ToString(),
                BoxCount = x.FlaggedUnits.GroupBy(g => g.OriginalBoxNumber).Count().ToString(),
                URL = "http://" + ConfigurationManager.AppSettings["BaseURL"] + "/Operator/ViewRequest/" + x.FlagRequestID,
                WarehouseName = x.WarehouseName,
                MRBID = x.MRBID

            };

            var parsedContent = etf.ParseEmail<FlagRequestCreatedEmailViewModel>("FlagRequestCreatedEmail.cshtml", emailTemplate);
            var _warehouse = uow.Warehouses.GetAll().FirstOrDefault(y => y.PlantCode == x.WarehouseName);
            var _primaryWarehouseEmails = _warehouse.PrimaryEmailAddresses.Split(';').ToList();
            var _ccWarehouseEmails = _warehouse.CCEmailAddresses == null ? new List<string>() : _warehouse.CCEmailAddresses.Split(';').ToList();


            //TASK8376818 : Email Changes 
            var _reqEmail = (user as ISEPrincipal).WUFTUser.IdSid;
            var _requestor = _personHelper.GetPersonModelByIdsid(_reqEmail).Email;

            if(_warehouse.PlantCode == "MYB9")
            { 
             _ccWarehouseEmails.Add(_requestor);// INC010289625 : Add requestor and in CC and remove Customer service from CC address
                if(_ccWarehouseEmails.Contains("customer.service.malaysia.myb0.myb8.myb9@intel.com"))
                     _ccWarehouseEmails.Remove("customer.service.malaysia.myb0.myb8.myb9@intel.com");
                if(_ccWarehouseEmails.Contains("customer.service.malaysia.myb8.myb1.myb0@intel.com"))
                     _ccWarehouseEmails.Remove("customer.service.malaysia.myb8.myb1.myb0@intel.com");
            }

            string requestorEmail = (user as ISEPrincipal).WUFTUser.EmailAddress;
            _ccWarehouseEmails.Add(requestorEmail);

            emailer.AddTo(_primaryWarehouseEmails);
            emailer.AddCc(_ccWarehouseEmails);
            //emailer.Send("New Warehouse " + emailTemplate.Disposition + " Request Created for " + emailTemplate.WarehouseName + ", MRB Number: " + emailTemplate.MRBID, parsedContent);
            //FS-INC100005770
            if (emailTemplate.Disposition.Equals("Demix for Scrap"))
            {
                emailer.Send("New WH Demix Request Created: MRB " + emailTemplate.MRBID + ", " + emailTemplate.WarehouseName + ", " + x.CreatedOn, parsedContent); 
            }
            else if(emailTemplate.Disposition.Equals("Release"))
            {
                emailer.Send("New Release Request for WH demix Created: MRB " + emailTemplate.MRBID + ", " + emailTemplate.WarehouseName, parsedContent); 
            }
            emailer.ResetEmail();
        }
        //Begin : FS-INC100005745 : Email to admin to notify conflicting boxes
        public void SendConflictEmails(FlagRequest _request, List<KeyValuePair<string, string>> ConflictBoxIDs, string newMRBID, WUFTUser newRequestor)
        {
            var _originalRequestor = _personHelper.GetPersonModelByIdsid(_request.CreatedBy);

            var emailTemplate = new FlagRequestConflictEmailViewModel
            {
                Disposition = uow.Dispositions.GetById(_request.DispositionID).DispositionName,                
                ConflictBoxID = ConflictBoxIDs.ToDictionary(x => x.Key, x => x.Value),
                OriginalRequestor = _originalRequestor.FullName,
                NewRequestor = newRequestor.FullName,
                WarehouseName = _request.WarehouseName,
                OriginalMRBID = _request.MRBID,
                NewMRBID = newMRBID,
                URL = "http://" + ConfigurationManager.AppSettings["BaseURL"] + "/QRE/ViewRequest/" + _request.FlagRequestID
            };

            var parsedContent = etf.ParseEmail<FlagRequestConflictEmailViewModel>("FlagRequestConflictBoxEmail.cshtml", emailTemplate);
            var _warehouse = uow.Warehouses.GetAll().FirstOrDefault(y => y.PlantCode == _request.WarehouseName);

            emailer.AddTo("qin.xu@intel.com");
            emailer.AddTo("benedict.zhang@intel.com");
          
            emailer.Send("Overlapping Box detected pre-prod!", parsedContent);
            emailer.ResetEmail();

            ////adding below code for temporary email sending test
            //string smtpServer = "OutlookBG.intel.com";
            //string smtpPort = "25";
            //System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
            //mailMessage.From = new System.Net.Mail.MailAddress("vikashx.sharma@intel.com");
            //mailMessage.To.Add("vikashx.sharma@intel.com");           
            //mailMessage.Subject = "Overlapping Box detected pre-prod!";
            //mailMessage.Body = parsedContent;
            //mailMessage.IsBodyHtml = true;

            //System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(smtpServer, int.Parse(smtpPort));
            //client.Send(mailMessage);


        }//End : FS-INC100005745 : Email to admin to notify conflicting boxes

        public void SendConflictEmail(FlagRequest _request, List<KeyValuePair<string,int>> ConflictVisualIDList,  string newMRBID, WUFTUser newRequestor)
        {
            var _originalRequestor = _personHelper.GetPersonModelByIdsid(_request.CreatedBy);

            var emailTemplate = new FlagRequestConflictEmailViewModel
            {
                Disposition = uow.Dispositions.GetById(_request.DispositionID).DispositionName,
                ConflictVisualIDList = ConflictVisualIDList.Select(s => s.Key).ToList(),               
                OriginalRequestor = _originalRequestor.FullName,
                NewRequestor = newRequestor.FullName,
                WarehouseName = _request.WarehouseName,
                OriginalMRBID = _request.MRBID,
                NewMRBID = newMRBID,
                URL = "http://" + ConfigurationManager.AppSettings["BaseURL"] + "/QRE/ViewRequest/" + _request.FlagRequestID
            };

            var parsedContent = etf.ParseEmail<FlagRequestConflictEmailViewModel>("FlagRequestConflictEmail.cshtml", emailTemplate);
            var _warehouse = uow.Warehouses.GetAll().FirstOrDefault(y => y.PlantCode == _request.WarehouseName);
                        
            string newRequestorEmail = newRequestor.EmailAddress;
            string oldRequestorEmail = _originalRequestor.Email;

            List<string> peepsToCC = uow.ErrorEmailCCs.GetAll().Select(s => s.EmailAddress).ToList();
            peepsToCC.Add(oldRequestorEmail);  

            emailer.AddTo(newRequestorEmail); 
            emailer.AddCc(peepsToCC);

            emailer.Send("Request Conflicts with Previous " + emailTemplate.Disposition + " Request Notification", parsedContent);
            emailer.ResetEmail();          

        }

        public void SendReminder(int id)
        {
            var _request = uow.FlagRequests.GetAll().IncludeMultiple(x => x.Warehouse, x => x.Disposition).FirstOrDefault(x => x.FlagRequestID == id);
            var _units = uow.ECD_WarehouseDemixes.GetAll().Where(x => x.FlagRequestID == id).Select(x => new { BoxID = x.OriginalBoxNumber, LotID = x.OriginalLotNumber }).ToList();
            var _requestor = (HttpContext.User as ISEPrincipal).WUFTUser;

            var emailTemplate = new FlagRequestReminderEmailViewModel();
            emailTemplate.Disposition = _request.Disposition.DispositionName;
            emailTemplate.FlaggedUnitCount = _units.Count.ToString();
            emailTemplate.LotCount = _units.Select(x => x.LotID).Distinct().Count().ToString();
            emailTemplate.BoxCount = _units.Select(x => x.BoxID).Distinct().Count().ToString();
            emailTemplate.URL = "http://" + ConfigurationManager.AppSettings["BaseURL"] + "/Operator/ViewRequest/" + _request.FlagRequestID;
            emailTemplate.WarehouseName = _request.WarehouseName;
            emailTemplate.MRBID = _request.MRBID;

            var parsedContent = etf.ParseEmail<FlagRequestReminderEmailViewModel>("FlagRequestReminderEmail.cshtml", emailTemplate);

            string requestorEmail = _requestor.EmailAddress;

            var _primaryWarehouseEmails = _request.Warehouse.PrimaryEmailAddresses.Split(';').ToList();
            var _ccWarehouseEmails = _request.Warehouse.CCEmailAddresses == null ? new List<string>() : _request.Warehouse.CCEmailAddresses.Split(';').ToList();
            _ccWarehouseEmails.Add(requestorEmail);
            emailer.AddTo(_primaryWarehouseEmails);
            emailer.AddCc(_ccWarehouseEmails);
            emailer.Send("Warehouse " + emailTemplate.Disposition + " Request Reminder for " + emailTemplate.WarehouseName + ", MRB Number: " + emailTemplate.MRBID, parsedContent);
            emailer.ResetEmail();
        }

        public void SendCompletedBoxEmail(int flagRequestID, string originalBoxNumber)
        {
            var _request = uow.FlagRequests.GetAll().Where(x => x.FlagRequestID == flagRequestID).IncludeMultiple(x => x.Disposition, x => x.FlaggedUnits, x => x.Warehouse).FirstOrDefault();

            var _flaggedBoxes = _request.FlaggedUnits.Where(x => x.OriginalBoxNumber == originalBoxNumber)
                .GroupBy(g => new
                {
                    NewBoxID = g.DestinationBoxNumber,
                    UnmergeLotID = g.DestinationLotNumber,
                    OldBoxID = g.OriginalBoxNumber,
                    OldLotID = g.OriginalLotNumber
                }).ToList().Select(x => new CompletedBoxSummary
                {
                    BadVisualIDCount = x.Count().ToString(),
                    BadBoxID = x.Key.NewBoxID,
                    UnmergeLotID = x.Key.UnmergeLotID,
                    GoodBoxID = x.Key.OldBoxID,
                    LotID = x.Key.OldLotID
                }).ToList();

            var emailTemplate = new FlagRequestCompletedEmailViewModel
            {
                Disposition = _request.Disposition.DispositionName,
                Warehouse = _request.WarehouseName,
                Requestor = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).FullName,
                Boxes = _flaggedBoxes,
                MRBID = _request.MRBID,
                Title = _request.Disposition.DispositionName == "Demix for Scrap" ? "Demix for Scrap Box Completed" : "Unmerge Box Completed"
            };
            emailTemplate.ViewURL += flagRequestID;
            emailTemplate.DownloadURL += flagRequestID;

            var parsedContent = etf.ParseEmail<FlagRequestCompletedEmailViewModel>("BoxCompletedEmail.cshtml", emailTemplate);

            var _requestorEmail = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).Email;
            var _ccWarehouseEmails = _request.Warehouse.CCEmailAddresses == null ? new List<string>() : _request.Warehouse.CCEmailAddresses.Split(';').ToList();

            emailer.AddTo(_requestorEmail);
            emailer.AddCc(_ccWarehouseEmails);
            //change made for FS-INC100004647 - changes reverted back as requested by bzhan10.
            //emailer.Send((emailTemplate.Disposition == "Demix for Scrap" ? "Demix for Scrap" : "Unmerge") + " bad box " + _flaggedBoxes.FirstOrDefault().BadBoxID + " just created, please On-Hold through ECD immediately to avoid escape", parsedContent);
            emailer.Send((emailTemplate.Disposition == "Demix for Scrap" ? "(WH Demix for Scrap)" : "Unmerge") + " New box " + _flaggedBoxes.FirstOrDefault().BadBoxID + " just created, please On-Hold through ECD to avoid escape", parsedContent);
            emailer.ResetEmail();
        }

        [Route("/Email/SendCompletedEmail/{id}")]
        public void SendCompletedEmail(int id)
        {
            var _request = uow.FlagRequests.GetAll().Where(x => x.FlagRequestID == id).IncludeMultiple(x => x.Disposition, x => x.FlaggedUnits, x => x.Warehouse).FirstOrDefault();

            var _goodQtys = uow.BoxRequestStatuses.GetAll().Where(x => x.FlagRequestID == id).Select(x => new
            {               
                FlagRequestID = x.FlagRequestID,
                BoxNumber = x.BoxNumber,
                Qty = x.GoodBoxUnitQty
            }).ToList();

            var _flaggedBoxes = _request.FlaggedUnits
                .GroupBy(g => new
                {
                    NewBoxID = g.DestinationBoxNumber,
                    UnmergeLotID = g.DestinationLotNumber,
                    OldBoxID = g.OriginalBoxNumber,
                    OldLotID = g.OriginalLotNumber
                }).ToList().Select(x => new CompletedBoxSummary
                {
                    BadVisualIDCount = x.Count().ToString(),
                    BadBoxID = x.Key.NewBoxID,
                    UnmergeLotID = x.Key.UnmergeLotID,
                    GoodBoxID = x.Key.OldBoxID,
                    LotID = x.Key.OldLotID,
                    GoodVisualIDCount = _goodQtys.FirstOrDefault(z => z.FlagRequestID == id && z.BoxNumber == x.Key.OldBoxID).Qty.ToString()
                }).ToList();

            var emailTemplate = new FlagRequestCompletedEmailViewModel
            {
                Disposition = _request.Disposition.DispositionName,
                Warehouse = _request.WarehouseName,
                Requestor = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).FullName,
                Boxes = _flaggedBoxes,
                MRBID = _request.MRBID,
                Title = (_request.Disposition.DispositionName == "Demix for Scrap" ? "Demix for Scrap" : "Unmerge") + " Request Completed"
            };
            emailTemplate.ViewURL += id;
            emailTemplate.DownloadURL += id;

            var parsedContent = etf.ParseEmail<FlagRequestCompletedEmailViewModel>("FlagRequestCompletedEmail.cshtml", emailTemplate);

            var _requestorEmail = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).Email;
            var _ccWarehouseEmails = _request.Warehouse.CCEmailAddresses == null ? new List<string>() : _request.Warehouse.CCEmailAddresses.Split(';').ToList();

            emailer.AddTo(_requestorEmail);
            emailer.AddCc(_ccWarehouseEmails);
            //emailer.Send("Warehouse " + (emailTemplate.Disposition == "Demix for Scrap" ? "Demix for Scrap" : "Unmerge") + " Request for " + emailTemplate.Warehouse + " has completed, MRB Number: " + emailTemplate.MRBID, parsedContent);
            emailer.Send("WH Demix request has completed: MRB " + emailTemplate.MRBID +", "+ emailTemplate.Warehouse+", "+ _request.CreatedOn, parsedContent);//FS-INC100005770
            emailer.ResetEmail();
        }

        [Route("/Email/SendCompletedWithMissingUnitsEmail/{id}")]
        public void SendCompletedWithMissingUnitsEmail(int id)
        {
            var _request = uow.FlagRequests.GetAll().Where(x => x.FlagRequestID == id).IncludeMultiple(x => x.Disposition, x => x.FlaggedUnits, x => x.Warehouse).FirstOrDefault();
            var _completedWithMissingStatus = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName.Equals("Completed with Units Missing")).RequestStatusID;
            var _goodQtys = uow.BoxRequestStatuses.GetAll().Where(x => x.FlagRequestID == id).Select(x => new
            {
                FlagRequestID = x.FlagRequestID,
                BoxNumber = x.BoxNumber,
                Qty = x.GoodBoxUnitQty
            }).ToList();

            var _flaggedBoxes = _request.FlaggedUnits
                .GroupBy(g => new
                {
                    NewBoxID = g.DestinationBoxNumber,
                    UnmergeLotID = g.DestinationLotNumber,
                    OldBoxID = g.OriginalBoxNumber,
                    OldLotID = g.OriginalLotNumber
                }).ToList().Select(x => new CompletedBoxSummary
                {
                    BadVisualIDCount = x.Count().ToString(),
                    BadBoxID = x.Key.NewBoxID,
                    UnmergeLotID = x.Key.UnmergeLotID,
                    GoodBoxID = x.Key.OldBoxID,
                    LotID = x.Key.OldLotID,
                    GoodVisualIDCount = _goodQtys.FirstOrDefault(z => z.FlagRequestID == id && z.BoxNumber == x.Key.OldBoxID).Qty.ToString()
                }).ToList();

            var _missingunits = _request.FlaggedUnits.Where(w => w.UnitFound == false && _request.RequestStatusID == _completedWithMissingStatus).Select(s => new LotBoxUnit
            {
                BoxNumber = s.OriginalBoxNumber,
                LotNumber = s.OriginalLotNumber,
                VisualID = s.SubstrateVisualID
            }).ToList();

            var _foundStoppedRequestUnits = _request.FlaggedUnits.Where(x => x.UnitFoundButRequestStopped).Select(x => new LotBoxUnit
            {
                BoxNumber = x.OriginalBoxNumber,
                LotNumber = x.OriginalLotNumber,
                VisualID = x.SubstrateVisualID
            }).ToList();

            var emailTemplate = new FlagRequestCompletedWithMissingUnitsEmailViewModel
            {
                Disposition = _request.Disposition.DispositionName,
                Warehouse = _request.WarehouseName,
                Requestor = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).FullName,
                Boxes = _flaggedBoxes,
                MissingUnits = _missingunits,
                MRBID = _request.MRBID,
                UnitsFoundOnStopRequest = _foundStoppedRequestUnits
            };
            emailTemplate.ViewURL += id;
            emailTemplate.DownloadURL += id;

            var parsedContent = etf.ParseEmail<FlagRequestCompletedWithMissingUnitsEmailViewModel>("FlagRequestCompletedWithMissingUnitsEmail.cshtml", emailTemplate);

            var _requestorEmail = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).Email;
            var _ccWarehouseEmails = _request.Warehouse.CCEmailAddresses == null ? new List<string>() : _request.Warehouse.CCEmailAddresses.Split(';').ToList();

            emailer.AddTo(_requestorEmail);
            emailer.AddCc(_ccWarehouseEmails);
            emailer.Send("Warehouse " + emailTemplate.Disposition + " Request for " + emailTemplate.Warehouse + " has closed w/ Missing Units, MRB Number: " + emailTemplate.MRBID, parsedContent);
            emailer.ResetEmail();
        }
    }
}