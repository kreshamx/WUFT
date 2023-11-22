using CsvHelper;
using Excel;
using Hangfire;
using SharedCodeLibrary.Email;
using SharedCodeLibrary.Email.RazorTemplate.Factories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WUFT.DATA;
using WUFT.EF;
using WUFT.MODEL;
using WUFT.NET.Mappers;
using WUFT.NET.Util;
using WUFT.NET.ViewModels.QRE;
using WUFT.NET.ViewModels.Shared;

namespace WUFT.NET.Controllers
{
    [ISESecurity(Roles = "AMR\\ISE Logistics_WUFT_QRE, AMR\\ISE Logistics_ISE_Developer, GAR\\FlexAppsSupport")]
    public class QREController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly PersonHelper _personHelper;
        private readonly IEmailer emailer;
        private readonly IEmailTemplateFactory etf;

        public QREController(IUnitOfWork uow, IEmailTemplateFactory etf, IEmailer emailer)
        {
            this.uow = uow;
            this.etf = etf;
            this.emailer = emailer;
            _personHelper = new PersonHelper(uow);
            RecurringJob.AddOrUpdate("Remove_Old_Uploads", () => DBMaintance.DeleteOldUploads(), Cron.Hourly);
        }

        #region INDEX
        //[ISESecurity(Roles = @"ISE Logistics_WUFT_QRE")]
        public ActionResult Index()
        {
            var _twoWeeksAgo = DateTime.UtcNow.AddDays(-14);
            var _boxRequestStatuses = uow.BoxRequestStatuses.GetAll().ToList();
            var _completedID = uow.RequestStatuses.GetAll().ToList();

            List<RequestLineItemViewModel> _requests = new List<RequestLineItemViewModel>();

            //Code changes updated for FS-INC100004633

            //var _completedID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName == "Completed").RequestStatusID;

            //var _requests = uow.FlagRequests.GetAll()
            //    .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
            //    .Where(x => x.FlaggedUnits.Count > 0
            //      && x.RequestStatusID == _completedID && !(x.CompletedOn < _twoWeeksAgo))
            //    .Select(x => new RequestLineItemViewModel
            //    {
            //        LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
            //        TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
            //        UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
            //        UnitQty = x.FlaggedUnits.Count,
            //        OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
            //        Disposition = x.Disposition.DispositionName,
            //        LastModified = x.LastModified,
            //        CreatedOn = x.CreatedOn,
            //        Requestor = x.CreatedBy,
            //        RequestID = x.FlagRequestID,
            //        MRBID = x.MRBID,
            //        Warehouse = x.WarehouseName
            //    }).ToList();

            for (int i = 0; i < _completedID.Count; i++)
            {
                if (_completedID[i].RequestStatusName == "In Process" || _completedID[i].RequestStatusName == "New Request")
                {

                    if (_completedID[i].RequestStatusName == "In Process")
                    {
                        var _r1 = uow.FlagRequests.GetAll()
                       .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                       .Where(x => x.FlaggedUnits.Count > 0 && x.RequestStatus.RequestStatusName == "In Process")
                       .Select(x => new RequestLineItemViewModel
                       {
                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                           TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                           UnitQty = x.FlaggedUnits.Count,
                           BoxQty = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList().Count,//vsharm6x : TASK7720599 
                           OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                           Disposition = x.Disposition.DispositionName,
                           LastModified = x.LastModified,
                           CreatedOn = x.CreatedOn,
                           Requestor = x.CreatedBy,
                           RequestID = x.FlagRequestID,
                           MRBID = x.MRBID,
                           Warehouse = x.WarehouseName,
                           OriginalMRB = x.OriginalMRB
                       }).ToList();
                        _requests.AddRange(_r1);

                    }
                    else
                    {
                        var _r1 = uow.FlagRequests.GetAll()
                       .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                       .Where(x => x.FlaggedUnits.Count > 0 && x.RequestStatus.RequestStatusName == "New Request")
                       .Select(x => new RequestLineItemViewModel
                       {
                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                           TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                           UnitQty = x.FlaggedUnits.Count,
                           BoxQty = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList().Count,//vsharm6x : TASK7720599 
                           OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                           Disposition = x.Disposition.DispositionName,
                           LastModified = x.LastModified,
                           CreatedOn = x.CreatedOn,
                           Requestor = x.CreatedBy,
                           RequestID = x.FlagRequestID,
                           MRBID = x.MRBID,
                           Warehouse = x.WarehouseName,
                           OriginalMRB = x.OriginalMRB
                       }).ToList();
                        _requests.AddRange(_r1);
                    }

                }
                else if (_completedID[i].RequestStatusName == "Completed" || _completedID[i].RequestStatusName == "Stopped with Units Missing" || _completedID[i].RequestStatusName == "Completed with Units Missing")
                {
                    if (_completedID[i].RequestStatusName == "Completed")
                    {
                        var _r2 = uow.FlagRequests.GetAll()
                                   .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                                   .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed") && !(x.CompletedOn < _twoWeeksAgo))
                                   .Select(x => new RequestLineItemViewModel
                                   {
                                       LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                       TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                       UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                       UnitQty = x.FlaggedUnits.Count,
                                       BoxQty = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList().Count,//vsharm6x : TASK7720599 
                                       OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                       Disposition = x.Disposition.DispositionName,
                                       LastModified = x.LastModified,
                                       CreatedOn = x.CreatedOn,
                                       Requestor = x.CreatedBy,
                                       RequestID = x.FlagRequestID,
                                       MRBID = x.MRBID,
                                       Warehouse = x.WarehouseName,
                                       OriginalMRB = x.OriginalMRB
                                   }).ToList();
                        _requests.AddRange(_r2);
                    }
                    else if (_completedID[i].RequestStatusName == "Stopped with Units Missing")
                    {
                        var _r2 = uow.FlagRequests.GetAll()
                                   .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                                   .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Stopped with Units Missing") && !(x.CompletedOn < _twoWeeksAgo))
                                   .Select(x => new RequestLineItemViewModel
                                   {
                                       LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                       TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                       UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                       UnitQty = x.FlaggedUnits.Count,
                                       BoxQty = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList().Count,//vsharm6x : TASK7720599 
                                       OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                       Disposition = x.Disposition.DispositionName,
                                       LastModified = x.LastModified,
                                       CreatedOn = x.CreatedOn,
                                       Requestor = x.CreatedBy,
                                       RequestID = x.FlagRequestID,
                                       MRBID = x.MRBID,
                                       Warehouse = x.WarehouseName,
                                       OriginalMRB = x.OriginalMRB
                                   }).ToList();
                        _requests.AddRange(_r2);
                    }
                    else
                    {
                        var _r2 = uow.FlagRequests.GetAll()
                                                          .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                                                          .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed with Units Missing") && !(x.CompletedOn < _twoWeeksAgo))
                                                          .Select(x => new RequestLineItemViewModel
                                                          {
                                                              LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                                              TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                                              UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                                              UnitQty = x.FlaggedUnits.Count,
                                                              BoxQty = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList().Count,//vsharm6x : TASK7720599 
                                                              OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                                              Disposition = x.Disposition.DispositionName,
                                                              LastModified = x.LastModified,
                                                              CreatedOn = x.CreatedOn,
                                                              Requestor = x.CreatedBy,
                                                              RequestID = x.FlagRequestID,
                                                              MRBID = x.MRBID,
                                                              Warehouse = x.WarehouseName,
                                                              OriginalMRB = x.OriginalMRB
                                                          }).ToList();
                        _requests.AddRange(_r2);
                    }

                }

            }


            List<string> allBoxes = _requests.SelectMany(x => x.TempBoxIDs).ToList();
            List<string> _duplicateBoxes = allBoxes.GroupBy(x => x).ToList().Where(x => x.Count() > 1).ToList().Select(x => x.Key).ToList();

            _requests.ForEach(x =>
            {
                x.Requestor = _personHelper.GetPersonModelByIdsid(x.Requestor).FullName;
                x.TempBoxIDs.ForEach(b =>
                {
                    if (_duplicateBoxes.Contains(b))
                    {
                        x.BoxIDs.Add(new Tuple<string, bool>(b, true));
                    }
                    else
                    {
                        x.BoxIDs.Add(new Tuple<string, bool>(b, false));
                    }
                });
            });
            //vsharm6x : Split rows into multiple if box Qty is more than 10 
            //var count = 0;
            //_requests.ForEach(x =>
            //{
            //    if (x.BoxQty > 5)
            //    {
            //        while (x.BoxIDs.Count > 0)
            //        {
            //            //var boxIds1 = x.BoxIDs.GetRange(0, (5 - (count + 1)));
            //            var boxIds2 = x.BoxIDs.GetRange((5 * count), (5 - (count + 1)));
            //        }
            //    }
            //});


            return View(new RequestIndexParentViewModel
            {
                Requests = _requests
            });

        }
        #endregion

        #region VIEW REQUEST
        [Route("/QRE/ViewRequest/{id}")]
        public ActionResult ViewRequest(int id)
        {
            var _request = uow.FlagRequests.GetAll()
                .IncludeMultiple(x => x.Disposition, x => x.RequestStatus)
                .Select(s => new
                {
                    MRBID = s.MRBID,
                    Disposition = s.Disposition,
                    s.WarehouseName,
                    s.FlagRequestID,
                    s.RequestStatus,
                    s.CreatedBy,
                    s.LastModified
                })
                .Where(w => w.FlagRequestID == id)
                .ToList()
                .FirstOrDefault();

            ViewRequestViewModel _model = new ViewRequestViewModel();

            if (_request == null)
            {
                _model.InvalidID = true;
            }
            else
            {
                var _boxRequestStatuses = uow.BoxRequestStatuses.GetAll().IncludeMultiple(x => x.RequestStatus).Where(x =>
                    x.FlagRequestID == id).ToList();
                _model.CreatedBy = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).FullName;
                _model.CreatedByEmail = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).Email;
                _model.LastModifiedOn = _request.LastModified;
                _model.MRBID = _request.MRBID;
                _model.Disposition = _request.Disposition.DispositionName;
                _model.Warehouse = _request.WarehouseName;
                _model.RequestID = id;
                _model.RequestStatus = _request.RequestStatus.RequestStatusName;
                _model.BoxCount = _boxRequestStatuses.Count.ToString();

                if (_model.RequestStatus == "New Request" || _model.RequestStatus == "In Process")
                {
                    var _requestUnits = uow.ECD_WarehouseDemixes.GetAll()
                            .Where(w => w.FlagRequestID == id)
                            .GroupBy(x => new { LotNumber = x.OriginalLotNumber, BoxNumber = x.OriginalBoxNumber, UnmergeLotNumber = x.DestinationLotNumber })
                            .Select(s => new
                            {
                                Count = s.Count().ToString(),
                                UnmergeLotNumber = s.Key.UnmergeLotNumber ?? string.Empty,
                                LotNumber = s.Key.LotNumber,
                                BoxNumber = s.Key.BoxNumber
                            });
                    _model.BoxCount = _requestUnits.Select(x => x.BoxNumber).Distinct().Count().ToString();

                    _model.FlaggedBoxes = uow.ECD_WarehouseDemixes.GetAll()
                    .Where(w => w.FlagRequestID == id)
                    .GroupBy(x => new { LotNumber = x.OriginalLotNumber, BoxNumber = x.OriginalBoxNumber, UnmergeLotNumber = x.DestinationLotNumber })
                    .Select(s => new FlaggedBoxLineItem
                    {
                        BadVisualIDCount = s.Count().ToString(),
                        UnmergeLotID = s.Key.UnmergeLotNumber ?? string.Empty,
                        LotID = s.Key.LotNumber,
                        GoodBoxID = s.Key.BoxNumber,
                        //NewBoxID = s.FirstOrDefault().DestinationBoxNumber == null ? string.Empty : s.FirstOrDefault().DestinationBoxNumber
                    }).ToList();

                    //Calculate % done
                    _model.FlaggedBoxes.ForEach(x => x.BoxRequestStatus = _boxRequestStatuses.FirstOrDefault(z => z.BoxNumber == x.GoodBoxID).RequestStatus.RequestStatusName);
                    var _totalBoxes = _model.FlaggedBoxes.Count;
                    var _numerator = (double)(_model.FlaggedBoxes.Where(x => x.BoxRequestStatus.Contains("Complete") || x.BoxRequestStatus.Contains("Units")).Count());
                    _model.PercentDone = _totalBoxes == 0 ? "0" : Math.Round(((_numerator / _totalBoxes) * 100), 0).ToString();

                }
                else //Completed boxes
                {
                    var _boxes = uow.ECD_WarehouseDemixes.GetAll().Where(x => x.FlagRequestID == id).ToList().GroupBy(x =>
                        new
                        {
                            NewBoxID = x.DestinationBoxNumber,
                            UnmergeLotID = x.DestinationLotNumber,
                            OldBoxID = x.OriginalBoxNumber,
                            OldLotID = x.OriginalLotNumber
                        }).ToList()
                        .Select(x => new FlaggedBoxLineItem
                        {
                            VisualIDs = x.Select(y => new KeyValuePair<string, string>(y.SubstrateVisualID, y.UnitFound ? "Found" : y.UnitFoundButRequestStopped ? "Located" : "Not Found")).ToList(),
                            BadVisualIDCount = x.Count().ToString(),
                            BadBoxID = x.Key.NewBoxID,
                            GoodBoxID = x.Key.OldBoxID,
                            LotID = x.Key.OldLotID,
                            UnmergeLotID = x.Key.UnmergeLotID
                        }).ToList();
                    _boxes.ForEach(x => x.BoxRequestStatus = _boxRequestStatuses.FirstOrDefault(z => z.BoxNumber == x.GoodBoxID).RequestStatus.RequestStatusName);
                    _model.PercentDone = "100";

                    _model.CompletedBoxes = _boxes;
                }

            }

            return View(_model);
        }
        #endregion

        #region CREATE
        public ActionResult Create()
        {
            int maxrowcount;
            int maxunits = Int32.TryParse(ConfigurationManager.AppSettings["MaxUploadRows"], out maxrowcount) ? maxrowcount : 0;

            return View(
                new CreateRequestParentViewModel
                {
                    UploadVM = new UploadRequestViewModel() { MaxUploadUnits = maxunits },
                    ConfirmVM = null,
                }
                );
        }

        [HttpPost]
        public ActionResult Create(ConfirmRequestViewModel model)
        {
            var resultModel = InsertUploads(model);

            if (resultModel.Success)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error", resultModel);
            }
        }

        [HttpPost]
        public ActionResult UploadFile(UploadRequestViewModel model)
        {
            var returnModel = new CreateRequestParentViewModel();
            var _units = new List<ECD_WarehouseDemix>();
            if (model.File != null && model.File.ContentLength > 0)
            {
                //Read input File
                var ext = Path.GetExtension(model.File.FileName);
                if (ext == ".xlsx" || ext == ".xls" || ext == ".csv")
                {
                    ConsumeFile(ref model, ref _units);
                    //Check for errors
                    CheckForErrors(_units).ForEach(x => model.ErrorList.Add(x));

                    //Add warehouses to UI
                    var warehouses = _units.Where(x => x.UploadedWarehouseName != null).Select(x => x.UploadedWarehouseName).Distinct().ToList();
                    model.AffectedWarehouses = String.Join(", ", warehouses);
                }
                else
                {
                    model.ErrorList.Add("File extenstion not supported.");
                }

                //If there are errors, stay on upload
                if (model.ErrorList.Count > 0)
                {
                    returnModel.UploadVM = model;
                    returnModel.ConfirmVM = null;
                }
                else
                {
                    var confirmVM = new ConfirmRequestViewModel();
                    confirmVM.MRBID = model.MRBID;
                    string qreloadjobid = (User as ISEPrincipal).WUFTUser.IdSid + DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");

                    confirmVM.QRELoadJobID = qreloadjobid;

                    using (var bulkCopy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["WUFTDbContext"].ConnectionString))
                    {
                        var table = new DataTable();
                        var props = typeof(QRELoad).GetProperties().ToArray();

                        foreach (var propertyInfo in props)
                        {
                            bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                            table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                        }

                        var values = new object[props.Length];

                        foreach (var unit in _units)
                        {
                            DataRow row = table.NewRow();

                            row["QRELoadJobID"] = qreloadjobid;
                            row["QRELoadUserName"] = (User as ISEPrincipal).WUFTUser.IdSid;
                            row["QRELoadDateTime"] = DateTime.UtcNow;
                            row["LotID"] = unit.OriginalLotNumber;
                            row["BoxID"] = unit.OriginalBoxNumber;
                            row["VisualID"] = unit.SubstrateVisualID;
                            row["Warehouse"] = unit.UploadedWarehouseName;
                            row["DispositionID"] = unit.DispositionID;
                            row["FPOLotID"] = unit.DestinationLotNumber;

                            table.Rows.Add(row);
                        }
                        bulkCopy.BatchSize = 2000;
                        bulkCopy.BulkCopyTimeout = 0;
                        bulkCopy.DestinationTableName = "dbo.QRELoads";
                        bulkCopy.WriteToServer(table);
                    }

                    _units.GroupBy(x => new { x.DispositionID, x.UploadedWarehouseName }).ToList().ForEach(x =>
                    {
                        confirmVM.UploadUnits.Add(new UploadUnits
                        {
                            BoxCount = x.Select(y => y.OriginalBoxNumber).Distinct().Count().ToString(),
                            LotCount = x.Select(y => y.OriginalLotNumber).Distinct().Count().ToString(),
                            UnmergeLotCount = x.Select(y => y.DestinationLotNumber).Where(y => !String.IsNullOrEmpty(y)).Distinct().Count().ToString(),
                            UnitQty = x.Count().ToString(),
                            WarehouseName = x.Key.UploadedWarehouseName,
                            Disposition = uow.Dispositions.GetById(x.Key.DispositionID).DispositionName,
                            CollapseIdentifier = x.Key.UploadedWarehouseName + "_" + (uow.Dispositions.GetById(x.Key.DispositionID).DispositionName == "Demix for Scrap" ? "Scrap" : uow.Dispositions.GetById(x.Key.DispositionID).DispositionName), //'code fix for FS-INC100004651'
                            Units = x.GroupBy(y => new { y.OriginalBoxNumber, y.OriginalLotNumber, y.DestinationLotNumber }).ToList().Select(z => new UploadUnitsLineItem
                            {
                                BoxID = z.Key.OriginalBoxNumber,
                                LotID = z.Key.OriginalLotNumber,
                                UnmergeLotID = z.Key.DestinationLotNumber,
                                UnitQty = z.Count().ToString(),
                            }).ToList()
                        });
                    });

                    returnModel.ConfirmVM = confirmVM;
                    returnModel.UploadVM = null;
                }
            }
            else
            {
                returnModel.UploadVM = model;
                returnModel.ConfirmVM = null;
            }

            return View("Create", returnModel);
        }

        private List<UploadUnits> GenerateUploadUnits(IEnumerable<IGrouping<dynamic, ECD_WarehouseDemix>> units)
        {
            List<UploadUnits> upldUnits = new List<UploadUnits>();
            units.ToList().ForEach(x =>
            {
                upldUnits.Add(new UploadUnits
                {
                    BoxCount = x.Select(y => y.OriginalBoxNumber).Distinct().Count().ToString(),
                    LotCount = x.Select(y => y.OriginalLotNumber).Distinct().Count().ToString(),
                    UnmergeLotCount = x.Select(y => y.DestinationLotNumber).Where(y => !String.IsNullOrEmpty(y)).Distinct().Count().ToString(),
                    UnitQty = x.Count().ToString(),
                    WarehouseName = x.Key.UploadedWarehouseName,
                    Disposition = uow.Dispositions.GetById(x.Key.DispositionID).DispositionName,
                    CollapseIdentifier = x.Key.UploadedWarehouseName + "_" + uow.Dispositions.GetById(x.Key.DispositionID).DispositionName + "_" + x.FirstOrDefault().OriginalBoxNumber,
                    Units = x.GroupBy(y => new { y.OriginalBoxNumber, y.OriginalLotNumber, y.DestinationLotNumber }).ToList().Select(z => new UploadUnitsLineItem
                    {
                        BoxID = z.Key.OriginalBoxNumber,
                        LotID = z.Key.OriginalLotNumber,
                        UnmergeLotID = z.Key.DestinationLotNumber,
                        UnitQty = z.Count().ToString(),
                    }).ToList()
                });
            });
            return upldUnits;
        }

        [HttpPost]
        public ActionResult ConfirmMerge(string qrejobid, string mrbid)
        {

            ConfirmRequestViewModel model = new ConfirmRequestViewModel();
            model.QRELoadJobID = qrejobid;
            model.MRBID = mrbid;
            model.Merge = true;
            ErrorViewModel errorModel = InsertUploads(model);
            return Json(Url.Action("Index", "QRE"));
        }


        public ErrorViewModel InsertUploads(ConfirmRequestViewModel model)
        {
            var _newRequests = new List<FlagRequest>();

            const string CHECKEMAILERRORMSG = "Unable to create request due to unit conflict(s).  Please check your email for details.";
            const string VIDSINPROCESSERRORMSG = "Unable to create request.  Some of the Visual IDs submitted were found in another request currently being processed.";
            const string BADERRORMSG = "Unable to create request due to an application error.  Please contact support if the problem persists.";
            const string EMAILFAILURE = "Request was created.  Emails could not be sent.  Please go to WUFT to send a reminder to the warehouse.";
            const string COMBINEREQUESTSMSG = "There is another request with the same Disposition and MRB. Would you like to combine your request with the previous request?";
            const string COMPLETEDUPREQUESTSMSG = "There is another request with the same Disposition, MRB, and units. No changes were made.";
            //const string COMPLETEDUPREQUESTSMSG = "Unable to create request due to unit conflict(s). Please remove them from your request.";
            const string DUPLICATEUNITERRMSG = "Unable to create request due to unit conflict(s) with existing requests. Please check your email for details and resolve the conflict with exisiting request owner offline.";

            ErrorViewModel errorModel = new ErrorViewModel();
            errorModel.SupportURL = ConfigurationManager.AppSettings["ISESupportURL"].ToString();
            errorModel.ProvideSupportURL = false;
            EmailController eController = new EmailController(uow, etf, emailer, User);
            string MRB = model.MRBID;
            errorModel.SubstrateLists = new Dictionary<string, string>();

            try
            {
                //SendConflictBoxEmail(model); // FS-INC100005745 - Email notification for overlapping box[Build 1.0.6615.6]  

                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["WUFTDbContext"].ConnectionString;
                SqlCommand sql = new SqlCommand("uspLoadNewUnits", conn);
                sql.CommandType = System.Data.CommandType.StoredProcedure;
                sql.CommandTimeout = 60;

                SqlParameter pJobID = new SqlParameter("@QRELoadJobID", SqlDbType.VarChar);
                pJobID.Value = model.QRELoadJobID;
                sql.Parameters.Add(pJobID);

                SqlParameter pUser = new SqlParameter("@QREUser", SqlDbType.VarChar);
                pUser.Value = (User as ISEPrincipal).WUFTUser.IdSid;
                sql.Parameters.Add(pUser);

                SqlParameter pMRBID = new SqlParameter("@MRBID", SqlDbType.Int);
                pMRBID.Value = model.MRBID;
                sql.Parameters.Add(pMRBID);

                SqlParameter pMerge = new SqlParameter("@Merge", SqlDbType.Bit);
                pMerge.Value = model.Merge;
                sql.Parameters.Add(pMerge);

                conn.Open();
                var reader = sql.ExecuteReader();

                List<int> ErrorCodes = new List<int>();

                while (reader.Read())
                {
                    ErrorCodes.Add(Int32.Parse(reader["ErrorCode"].ToString()));
                }

                reader.NextResult();

                Dictionary<string, int> Duplicates = new Dictionary<string, int>();
                List<int> FlagRequestIDs = new List<int>();


                switch (ErrorCodes.FirstOrDefault())
                {
                    case 0:
                        List<int> FlagReqIDs = new List<int>();

                        while (reader.Read())
                        {
                            FlagReqIDs.Add(Int32.Parse(reader["FlagRequestID"].ToString()));
                        }
                        reader.Close();
                        _newRequests = uow.FlagRequests.GetAll().IncludeMultiple(i => i.FlaggedUnits).Where(w => FlagReqIDs.Contains(w.FlagRequestID)).ToList();
                        CreateBoxRequestStatuses(_newRequests);

                        try
                        {
                            if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmails"]) == true)
                            {
                                for (var i = 0; i < _newRequests.Count; i++)
                                {
                                    eController.SendCreateEmail(_newRequests[i]);
                                }
                            }
                            errorModel.Success = true;
                        }
                        catch (Exception)
                        {
                            errorModel.Message = EMAILFAILURE;
                            errorModel.Success = false;
                        }
                        return errorModel;
                    case 5000:

                        while (reader.Read())
                        {
                            Duplicates.Add(reader["SubstrateVisualID"].ToString(), Int32.Parse(reader["FlagRequestID"].ToString()));
                        }
                        reader.Close();

                        FlagRequestIDs = Duplicates.Select(s => s.Value).Distinct().ToList();
                        _newRequests = uow.FlagRequests.GetAll().IncludeMultiple(i => i.FlaggedUnits).Where(w => FlagRequestIDs.Contains(w.FlagRequestID)).ToList();
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmails"]) == true)
                        {
                            _newRequests.ForEach(x => eController.SendConflictEmail(x, Duplicates.Where(w => w.Value == x.FlagRequestID).ToList(), model.MRBID, (User as ISEPrincipal).WUFTUser));
                        }
                        errorModel.Success = false;
                        errorModel.Message = CHECKEMAILERRORMSG;
                        errorModel.ErrorCode = 5000;
                        return errorModel;
                    case 5001:

                        while (reader.Read())
                        {
                            Duplicates.Add(reader["SubstrateVisualID"].ToString(), Int32.Parse(reader["MRBID"].ToString()));
                        }
                        reader.Close();

                        FlagRequestIDs = Duplicates.Select(s => s.Value).Distinct().ToList();
                        errorModel.Success = false;
                        errorModel.Message = VIDSINPROCESSERRORMSG;
                        errorModel.SubstrateList = Duplicates.Take(50).ToDictionary(t => t.Key, t => t.Value);
                        errorModel.ErrorCode = 5001;
                        return errorModel;
                    case 6000:
                        {
                            var box = "";
                            int dispositionID = -1;
                            while (reader.Read())
                            {
                                Duplicates.Add(reader["SubstrateVisualID"].ToString(), Int32.Parse(reader["MRBID"].ToString()));
                                box = reader["BoxID"].ToString();
                                dispositionID = Int32.Parse(reader["Disposition"].ToString());
                            }
                            reader.Close();
                            FlagRequestIDs = Duplicates.Select(s => s.Value).Distinct().ToList();
                            errorModel.Success = false;
                            errorModel.BoxNumber = box;
                            errorModel.MRBID = model.MRBID;
                            errorModel.QRELoadJobID = model.QRELoadJobID;
                            errorModel.Message = COMBINEREQUESTSMSG;
                            errorModel.SubstrateList = Duplicates.Take(50).ToDictionary(t => t.Key, t => t.Value);
                            errorModel.ErrorCode = 6000;
                            errorModel.Disposition = dispositionID == -1 ? string.Empty : uow.Dispositions.GetAll().FirstOrDefault(x => x.DispositionID == dispositionID).DispositionName;
                            return errorModel;
                        }
                    case 7000:
                        {
                            var box = "";
                            int dispositionID = -1;
                            while (reader.Read())
                            {
                                Duplicates.Add(reader["SubstrateVisualID"].ToString(), Int32.Parse(reader["MRBID"].ToString()));
                                box = reader["BoxID"].ToString();
                                dispositionID = Int32.Parse(reader["Disposition"].ToString());
                            }
                            reader.Close();
                            FlagRequestIDs = Duplicates.Select(s => s.Value).Distinct().ToList();
                            errorModel.Success = false;
                            errorModel.BoxNumber = box;
                            errorModel.MRBID = model.MRBID;
                            errorModel.QRELoadJobID = model.QRELoadJobID;
                            errorModel.Message = COMPLETEDUPREQUESTSMSG;
                            errorModel.SubstrateList = Duplicates.Take(50).ToDictionary(t => t.Key, t => t.Value);
                            errorModel.ErrorCode = 7000;
                            errorModel.Disposition = dispositionID == -1 ? string.Empty : uow.Dispositions.GetAll().FirstOrDefault(x => x.DispositionID == dispositionID).DispositionName;
                            return errorModel;
                        }
                    default:
                        errorModel.Success = false;
                        errorModel.ProvideSupportURL = true;
                        errorModel.Message = BADERRORMSG;
                        return errorModel;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                errorModel.AdditionalMessage = e.InnerException != null ? (e.InnerException.InnerException != null ? e.InnerException.InnerException.Message : e.InnerException.Message) : e.Message;
                errorModel.Success = false;
                errorModel.ErrorCode = -1;
                errorModel.ProvideSupportURL = true;
                errorModel.Message = BADERRORMSG;
                return errorModel;
            }


        }

        //Begin : FS-INC100005745 - Email notification for overlapping box[Build 1.0.6615.6] 
        private void SendConflictBoxEmail(ConfirmRequestViewModel model)
        {
            var _newRequests = new List<FlagRequest>();
            ErrorViewModel errorModel = new ErrorViewModel();
            errorModel.SupportURL = ConfigurationManager.AppSettings["ISESupportURL"].ToString();
            errorModel.ProvideSupportURL = false;
            EmailController eController = new EmailController(uow, etf, emailer, User);
            string MRB = model.MRBID;
            const string DUPLICATEUNITERRMSG = "Unable to create request due to box conflict(s) with existing requests. Please check your email for details and resolve the conflict with exisiting request owner offline.";

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["WUFTDbContext"].ConnectionString;
            SqlCommand sqlN = new SqlCommand("uspFindMultipleDupUnits", conn);
            sqlN.CommandType = System.Data.CommandType.StoredProcedure;
            sqlN.CommandTimeout = 60;

            SqlParameter dMRBID = new SqlParameter("@MRBID", SqlDbType.Int);
            dMRBID.Value = model.MRBID;
            sqlN.Parameters.Add(dMRBID);

            int xCount;
            int errCount = 0;
            DataTable dt = new DataTable();
            conn.Open();

            dt.Load(sqlN.ExecuteReader());
            //xCount = dt.Rows.Count;
            xCount = (dt.Rows.Count > 0) ? Int32.Parse(dt.Rows[0].ItemArray[3].ToString()) : 0;

            conn.Close();
            List<int> flagReqIDs = new List<int>();

            //vsharm6x : changes of email notification for conflict in Box/Units 
            Dictionary<string, string> duplicateBOX = new Dictionary<string, string>();

            errorModel.SubstrateLists = new Dictionary<string, string>();

            string unitsBOX = string.Empty;
            string unitsVID = string.Empty;

            if (xCount > 1)
            {
                int boxIds = 0;

                foreach (DataRow dr in dt.Rows)
                {

                    if (dr[0].ToString().Contains("_Box"))
                    {
                        duplicateBOX.Add(dr[0].ToString().Split('_')[0] + "_B" + boxIds, dr[1].ToString());
                        errorModel.Message = DUPLICATEUNITERRMSG;
                        boxIds++;
                    }

                    flagReqIDs.Add(Int32.Parse(dr["FlagRequestID"].ToString()));

                    errCount++;

                }
                //List box id and visual ids as table
                var conflicts = from box in duplicateBOX
                                select new { BoxID = box.Key, UserID = box.Value };


                string oldKey = string.Empty;
                string oldVal = string.Empty;
                foreach (var req in conflicts)
                {
                    if (!(oldKey.Equals(req.BoxID.ToString().Split('_')[0]) && oldVal.Equals(req.UserID.ToString().Split('_')[0])))
                    {
                        errorModel.SubstrateLists.Add(req.BoxID.ToString().Split('_')[0], req.UserID.ToString().Split('_')[0]);
                    }
                    oldKey = req.BoxID.ToString().Split('_')[0];
                    oldVal = req.UserID.ToString().Split('_')[0];
                }
                // Send email notifications on conflicts of box ids or visual ids
                List<int> flagRequestIDs = new List<int>();
                _newRequests = uow.FlagRequests.GetAll().IncludeMultiple(i => i.FlaggedUnits).Where(w => flagReqIDs.Contains(w.FlagRequestID)).ToList();

                //flagRequestIDs = duplicates.Select(s => s.Value).Distinct().ToList();

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["SendEmails"]) == true)
                {
                    _newRequests.ForEach(x => eController.SendConflictEmails(x, errorModel.SubstrateLists.ToList(), model.MRBID, (User as ISEPrincipal).WUFTUser));
                }

            }

        }   ///End : FS-INC100005745 - Email notification for overlapping box[Build 1.0.6615.6]  

        private void CreateBoxRequestStatuses(List<FlagRequest> _newRequests)
        {
            int ID = 0;
            string str = null;

            foreach (FlagRequest item in _newRequests)
            {
                ID = item.DispositionID;
                //var oBoxID = item.FlaggedUnits.GroupBy(x => new {x.OriginalBoxNumber }).ToList();
                foreach (ECD_WarehouseDemix item1 in item.FlaggedUnits)
                {
                    str = item1.OriginalBoxNumber;
                }

            }

            //Update _processID as "Completed"(2) when disposition is 9

            if (ID == 9)
            {
                //SP call made by rshanm3x to remove duplicate BoxRequest on Release process. Fix for FS-INC100004335
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["WUFTDbContext"].ConnectionString;
                SqlCommand sql = new SqlCommand("removeDuplicateBoxRequestOnRelease", conn);
                sql.CommandType = System.Data.CommandType.StoredProcedure;
                sql.CommandTimeout = 60;

                SqlParameter sParam = new SqlParameter("@originalBoxNum", SqlDbType.VarChar);
                sParam.Value = str;
                sql.Parameters.Add(sParam);

                conn.Open();
                var reader = sql.ExecuteReader();

                var _processID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName.Equals("Completed")).RequestStatusID;
                _newRequests.ForEach(request =>
                {
                    var _boxIDs = request.FlaggedUnits.GroupBy(x => new { x.OriginalBoxNumber, x.OriginalLotNumber, x.DestinationLotNumber })
                        .ToList();
                    _boxIDs.ForEach(box =>
                    {
                        uow.BoxRequestStatuses.Add(new BoxRequestStatus
                        {
                            LastModifiedBy = "wuft",
                            BoxNumber = box.Key.OriginalBoxNumber,
                            RequestStatusID = _processID,
                            FlagRequestID = request.FlagRequestID,
                            CreatedOn = DateTime.UtcNow,
                            LastModifiedOn = DateTime.UtcNow
                        });
                    });
                });
            }
            else
            {
                var _processID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName.Equals("New Request")).RequestStatusID;
                _newRequests.ForEach(request =>
                {
                    var _boxIDs = request.FlaggedUnits.GroupBy(x => new { x.OriginalBoxNumber, x.OriginalLotNumber, x.DestinationLotNumber })
                        .ToList();
                    _boxIDs.ForEach(box =>
                    {
                        uow.BoxRequestStatuses.Add(new BoxRequestStatus
                        {
                            LastModifiedBy = "wuft",
                            BoxNumber = box.Key.OriginalBoxNumber,
                            RequestStatusID = _processID,
                            FlagRequestID = request.FlagRequestID,
                            CreatedOn = DateTime.UtcNow,
                            LastModifiedOn = DateTime.UtcNow
                        });
                    });
                });
            }

            //_newRequests.ForEach(request =>
            //{
            //    var _boxIDs = request.FlaggedUnits.GroupBy(x=> new { x.OriginalBoxNumber, x.OriginalLotNumber, x.DestinationLotNumber})
            //        .ToList();
            //    _boxIDs.ForEach(box =>
            //    {
            //        uow.BoxRequestStatuses.Add(new BoxRequestStatus
            //        {
            //            LastModifiedBy = "wuft",
            //            BoxNumber = box.Key.OriginalBoxNumber,
            //            RequestStatusID = _processID,
            //            FlagRequestID = request.FlagRequestID,
            //            CreatedOn = DateTime.UtcNow,
            //            LastModifiedOn = DateTime.UtcNow
            //        });
            //    });
            //});

            uow.SaveChanges();
        }

        private List<FlagRequest> getxlist()
        {
            throw new NotImplementedException();
        }

        private void ConsumeFile(ref UploadRequestViewModel model, ref List<ECD_WarehouseDemix> _units)
        {
            DataSet result = null;
            //List<ECD_WarehouseDemix> _units = new List<ECD_WarehouseDemix>();
            var _file = model.File;

            var _fileName = Path.GetFileName(model.File.FileName);

            //Get path name
            model.FilePath = Path.Combine(ConfigurationManager.AppSettings["FileUploadLocation"], _fileName);

            //check if file with the same name already exists and delete it
            if (System.IO.File.Exists(model.FilePath))
            {
                System.IO.File.Delete(model.FilePath);
            }

            //store file
            _file.SaveAs(model.FilePath);

            var ext = Path.GetExtension(model.FilePath);
            if (ext == ".csv")
            {
                try
                {
                    //TODO: Test CSV
                    _units = ReadCsv(model.FilePath, _units);
                    _units = CheckWarehouses(_units);
                }
                catch (CsvHelper.CsvMissingFieldException e)
                {
                    model.ErrorList.Add(e.Message);
                }
            }
            else if (ext == ".xlsx" || ext == ".xls")
            {
                if (ext == ".xlsx")
                {
                    result = ReadXlsx(model.FilePath, result);
                }
                else if (ext == ".xls")
                {
                    result = ReadXls(model.FilePath, result);

                }
                try
                {

                    //Validate Columns
                    List<string> columnHeaders = new List<string>();
                    var correctHeaders = true;

                    foreach (DataColumn col in result.Tables[0].Columns)
                    {
                        columnHeaders.Add(col.ColumnName);
                    }


                    List<string> errors = CheckColumnHeaders(columnHeaders);
                    if (errors.Count == 0)
                        correctHeaders = true;
                    else
                    {
                        correctHeaders = false;
                        foreach (string error in errors)
                        {
                            model.ErrorList.Add(error);
                        }
                    }

                    if (correctHeaders)
                    {
                        try
                        {
                            foreach (DataRow rec in result.Tables[0].Rows)
                            {
                                int dispositionID;
                                Int32.TryParse(rec["DISPOSITION"].ToString(), out dispositionID);

                                ECD_WarehouseDemix wd = new ECD_WarehouseDemix();

                                wd.OriginalLotNumber = rec["LOT ID"].ToString().Trim();
                                wd.OriginalBoxNumber = rec["BOX ID"].ToString().Trim();
                                wd.SubstrateVisualID = rec["VISUAL ID"].ToString().Trim();
                                wd.DestinationLotNumber = rec["FPO LOT ID"].ToString().Trim();
                                wd.UploadedWarehouseName = rec["WAREHOUSE"].ToString().Trim();
                                wd.DispositionID = dispositionID;
                                wd.CreatedOn = DateTime.UtcNow;
                                wd.LastUpdateOn = DateTime.UtcNow;
                                wd.StartDateTime = DateTime.UtcNow;
                                wd.EndDateTime = DateTime.UtcNow;
                                wd.CreatedBy = (User as ISEPrincipal).WUFTUser.IdSid;
                                wd.LastUpdateBy = (User as ISEPrincipal).WUFTUser.IdSid;
                                _units.Add(wd);

                                _units = CheckWarehouses(_units);
                            }
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException != null)
                                model.ErrorList.Add(e.InnerException.Message);
                            else if (e.Message != null)
                                model.ErrorList.Add(e.Message);
                        }

                    }

                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        model.ErrorList.Add(e.InnerException.Message);
                    else if (e.Message != null)
                        model.ErrorList.Add(e.Message);
                }
            }
        }
        #endregion

        #region VALIDATION CHECKS
        private List<string> CheckColumnHeaders(List<string> columnHeaders)
        {
            List<string> columnErrors = new List<string>();
            CheckForColumn(ref columnErrors, columnHeaders, "DISPOSITION");
            CheckForColumn(ref columnErrors, columnHeaders, "LOT ID");
            CheckForColumn(ref columnErrors, columnHeaders, "BOX ID");
            CheckForColumn(ref columnErrors, columnHeaders, "VISUAL ID");
            CheckForColumn(ref columnErrors, columnHeaders, "WAREHOUSE");
            CheckForColumn(ref columnErrors, columnHeaders, "FPO LOT ID");
            return columnErrors;
        }

        private List<string> CheckForErrors(List<ECD_WarehouseDemix> _units)
        {
            List<string> _errorList = new List<string>();

            //Check warehouses validity
            CheckForValidWarehouses(_units, ref _errorList);

            //Check all Visual IDs are populated
            CheckForBlankVisualIDs(_units, ref _errorList);

            //Check all Lot IDs are populated
            CheckForBlankLotIDs(_units, ref _errorList);

            //Check all FPO LOT IDs are populated for disposition 7
            CheckForBlankFPOLotIDsOnUnmerge(_units, ref _errorList);

            //Check that no FPO LOT IDs filled in for disposition 8
            CheckForFilledInFPOLotIDsOnScrap(_units, ref _errorList);

            //Check all Box IDs are populated
            CheckForBlankBoxIDs(_units, ref _errorList);

            //Check all Warehouses are populated
            CheckForBlankWarehouses(_units, ref _errorList);

            //Check all Dispositions are populated
            CheckForBlankDispositions(_units, ref _errorList);

            //Check all Dispositions are valid
            CheckForValidDispositions(_units, ref _errorList);

            //Check for duplicate Visual IDs
            CheckForDuplicates(_units, ref _errorList);

            //Check if row count exceeds max number of rows
            if (CheckForRowCount(_units))
            {
                int maxrowcount;
                bool result = Int32.TryParse(ConfigurationManager.AppSettings["MaxUploadRows"], out maxrowcount);
                if (result)
                {
                    _errorList.Add("The maximum number of rows has been exceeded. The maximum number of rows is " + maxrowcount.ToString("N0") + ".");
                }
                else
                {
                    _errorList.Add("The maximum number of rows has been exceeded.");
                }
            }

            return _errorList;
        }

        private void CheckForValidWarehouses(List<ECD_WarehouseDemix> _units, ref List<string> _errorList)
        {
            if (_units.Any(x => x.UploadedWarehouseName == null))
                _errorList.Add("One or more Warehouses were not recognized.");
        }

        private void CheckForBlankFPOLotIDsOnUnmerge(List<ECD_WarehouseDemix> _units, ref List<string> _errorList)
        {
            if (_units.Where(x => x.DispositionID == 7).ToList().Any(x => String.IsNullOrEmpty(x.DestinationLotNumber)))
                _errorList.Add("One or more FPO Lot IDs were left blank on an UNMERGE request.");
        }

        private void CheckForFilledInFPOLotIDsOnScrap(List<ECD_WarehouseDemix> _units, ref List<string> _errorList)
        {
            if (_units.Where(x => x.DispositionID == 8).ToList().Any(x => !String.IsNullOrEmpty(x.DestinationLotNumber)))
                _errorList.Add("One or more FPO Lot IDs were filled in on a SCRAP request.");
        }

        private bool CheckForRowCount(List<ECD_WarehouseDemix> _units)
        {
            int maxrowcount;
            return (_units.Count > (Int32.TryParse(ConfigurationManager.AppSettings["MaxUploadRows"], out maxrowcount) ? maxrowcount : 20000)).Equals(true);
        }

        private void CheckForDuplicates(List<ECD_WarehouseDemix> _units, ref List<string> _errorList)
        {
            if (_units.GroupBy(g => g.SubstrateVisualID).Where(g => g.Count() > 1).Select(s => s.Key).Count() >= 1)
                _errorList.Add("One or more Visual IDs were duplicated.");
        }

        private void CheckForBlankDispositions(List<ECD_WarehouseDemix> _units, ref List<string> _errorList)
        {
            if (_units.Any(x => x.DispositionID == 0))
                _errorList.Add("One or more Dispositions were left blank.");
        }

        private void CheckForValidDispositions(List<ECD_WarehouseDemix> _units, ref List<string> _errorList)
        {
            var dispoIDs = uow.Dispositions.GetAll().Select(x => x.DispositionID).ToList();
            if (_units.Any(x => !dispoIDs.Contains(x.DispositionID)))
                _errorList.Add("One or more Dispositions IDs were invalid.");
        }

        private void CheckForBlankWarehouses(List<ECD_WarehouseDemix> _units, ref List<string> _errorList)
        {
            if (_units.Any(x => x.UploadedWarehouseName == String.Empty))
                _errorList.Add("One or more Warehouses were left blank.");
        }

        private void CheckForBlankBoxIDs(List<ECD_WarehouseDemix> _units, ref List<string> _errorList)
        {
            if (_units.Any(x => String.IsNullOrEmpty(x.OriginalBoxNumber)))
                _errorList.Add("One or more Box IDs were left blank.");
        }

        private void CheckForBlankLotIDs(List<ECD_WarehouseDemix> _units, ref List<string> _errorList)
        {
            if (_units.Any(x => String.IsNullOrEmpty(x.OriginalLotNumber)))
                _errorList.Add("One or more Lot IDs were left blank.");
        }

        private void CheckForBlankVisualIDs(List<ECD_WarehouseDemix> _units, ref List<string> _errorList)
        {
            if (_units.Any(x => String.IsNullOrEmpty(x.SubstrateVisualID)))
                _errorList.Add("One or more Visual IDs were left blank.");
        }

        private void CheckForColumn(ref List<string> _errors, List<string> _headers, string _columnName)
        {
            var count = _headers.Where(x => x.StartsWith(_columnName)).Count();
            if (count == 0)
                _errors.Add("Missing " + _columnName + " column.");
            else if (count > 1)
                _errors.Add("Duplicate " + _columnName + " columns found.");
        }
        #endregion

        #region EXCEL FILE READERS
        private List<ECD_WarehouseDemix> ReadCsv(string _filePath, List<ECD_WarehouseDemix> _units)
        {
            using (TextReader textReader = new StreamReader(_filePath))
            {
                var csv = new CsvReader(textReader);
                csv.Configuration.RegisterClassMap<ECDWarehouseDemix_CSVMapper>();
                _units = csv.GetRecords<ECD_WarehouseDemix>().ToList();
                _units.ToList().ForEach(x =>
                {
                    x.LastUpdateBy = (User as ISEPrincipal).WUFTUser.IdSid;
                    x.CreatedBy = (User as ISEPrincipal).WUFTUser.IdSid;
                });
            }
            return _units;
        }

        private static DataSet ReadXls(string _filePath, DataSet result)
        {
            FileStream stream = System.IO.File.Open(_filePath, FileMode.Open, FileAccess.ReadWrite);
            IExcelDataReader xlsReader = ExcelReaderFactory.CreateBinaryReader(stream);
            xlsReader.IsFirstRowAsColumnNames = true;
            result = xlsReader.AsDataSet();
            xlsReader.Close();
            return result;
        }

        private static DataSet ReadXlsx(string _filePath, DataSet result)
        {
            FileStream stream = System.IO.File.Open(_filePath, FileMode.Open, FileAccess.ReadWrite);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            excelReader.IsFirstRowAsColumnNames = true;
            result = excelReader.AsDataSet();
            excelReader.Close();
            return result;
        }
        #endregion

        #region HELPER FUNCTIONS
        public List<ECD_WarehouseDemix> CheckWarehouses(List<ECD_WarehouseDemix> _units)
        {
            List<Warehouse> warehouses = uow.Warehouses.GetAll().ToList();

            //Check that all warehouses are in the DB
            _units.Where(x => !String.IsNullOrEmpty(x.UploadedWarehouseName)).ToList().ForEach(x =>
            {
                if (warehouses.Where(y => y.PlantCode == x.UploadedWarehouseName).FirstOrDefault() == null)
                    x.UploadedWarehouseName = null;
            });
            return _units;
        }

        #endregion
    }
}