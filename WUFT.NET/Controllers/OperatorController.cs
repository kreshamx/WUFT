using SharedCodeLibrary.Email;
using SharedCodeLibrary.Email.RazorTemplate.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WUFT.DATA;
using WUFT.EF;
using WUFT.NET.Util;
using WUFT.NET.ViewModels.Operator;
using WUFT.NET.ViewModels.Shared;

namespace WUFT.NET.Controllers
{
    [ISESecurity(Roles = "AMR\\ISE Logistics_WUFT_Operator, AMR\\ISE Logistics_WUFT_Admin, AMR\\ISE Logistics_WUFT_QRE, GAR\\FlexAppsSupport")]
    public class OperatorController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly IEmailTemplateFactory etf;
        private readonly IEmailer emailer;
        private readonly PersonHelper _personHelper;

        public OperatorController(IUnitOfWork uow, IEmailTemplateFactory etf, IEmailer emailer)
        {
            this.uow = uow;
            this.etf = etf;
            this.emailer = emailer;
            _personHelper = new PersonHelper(uow);
        }

        public JsonResult CheckMissingUnits(int id)
        {
            List<string> _missingUnits = uow.ECD_WarehouseDemixes.GetAll().Where(x => x.FlagRequestID == id && !x.UnitFound).Select(x => x.SubstrateVisualID).ToList();
            if (_missingUnits.Count == 0)
            {
                return Json(string.Empty, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var maxListSize = 20;
                if (_missingUnits.Count > maxListSize)
                {
                    return Json("More than " + maxListSize + " units are missing.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(String.Join("</br>", _missingUnits), JsonRequestBehavior.AllowGet);
                }
            }
        }

        private void EmailRequestor(List<string> _missingUnits, int id)
        {
            EmailController eController = new EmailController(uow, etf, emailer, User);
            if (_missingUnits.Count == 0)
            {
                eController.SendCompletedEmail(id);
            }
            else
            {
                eController.SendCompletedWithMissingUnitsEmail(id);
            }
        }

        public ActionResult Index()
        {
            var _warehouses = new List<SelectListItem> { new SelectListItem() { Value = "0", Text = "Show All" } };
            uow.Warehouses.GetAll().ToList().ForEach(x => _warehouses.Add(new SelectListItem() { Value = x.PlantCode, Text = x.PlantCode }));

            var _twoWeeksAgo = DateTime.UtcNow.AddDays(-14);

            //Code changes updated for FS-INC100004633
            var _completedID = uow.RequestStatuses.GetAll().ToList();
            List<RequestLineItemViewModel> _requests = new List<RequestLineItemViewModel>();
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

            //var _requests = uow.FlagRequests.GetAll()
            //    .Where(x => x.FlaggedUnits.Count > 0 && !(x.RequestStatus.RequestStatusName == "Complete" && x.LastModified > _twoWeeksAgo) && x.DispositionID != 9)
            //    .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition, x => x.RequestStatus)
            //    .Select(x => new RequestLineItemViewModel
            //    {
            //        LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
            //        TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
            //        UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
            //        UnitQty = x.FlaggedUnits.Count,
            //        OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
            //        Disposition = x.Disposition.DispositionName,
            //        CreatedOn = x.CreatedOn,
            //        Requestor = x.CreatedBy,
            //        RequestID = x.FlagRequestID,
            //        MRBID = x.MRBID,
            //        Warehouse = x.WarehouseName
            //    }).ToList();

            List<string> allBoxes = _requests.SelectMany(x => x.TempBoxIDs).ToList();
            List<string> _duplicateBoxes = allBoxes.GroupBy(x => x).ToList().Where(x => x.Count() > 1).ToList().Select(x => x.Key).ToList();
            
            _requests.ForEach(x =>
            {
                x.Requestor = _personHelper.GetPersonModelByIdsid(x.Requestor).FullName;
                x.TempBoxIDs.ForEach(b => {
                    if(_duplicateBoxes.Contains(b))
                    {
                        x.BoxIDs.Add(new Tuple<string, bool>(b, true));
                    }
                    else
                    {
                        x.BoxIDs.Add(new Tuple<string, bool>(b, false));
                    }
                });
            });

            var _returnModel = new RequestIndexParentViewModel();
            _returnModel.Requests = _requests;
            //Setup warehouses
            if (HttpContext.Request.Cookies["warehouse"] == null)
            {
                _returnModel.SelectedWarehouse = "0"; //show all
            }
            else
            {
                var _userWarehouse = HttpContext.Request.Cookies["warehouse"].Value;
                if (_userWarehouse.Equals("undefined") || _userWarehouse.Equals("0"))
                {
                    _returnModel.SelectedWarehouse = "0"; //show all
                }
                else
                {
                    _returnModel.SelectedWarehouse = _warehouses.FirstOrDefault(x => x.Text == _userWarehouse).Value;
                }
            }
            _returnModel.Warehouses = _warehouses;

            return View(_returnModel);
        }

        public ActionResult ViewRequest(int id)
        {
            var _request = uow.FlagRequests.GetAll()
                .IncludeMultiple(x => x.Disposition, x => x.RequestStatus)
                .Select(s => new
                {
                    s.CreatedBy,
                    s.CreatedOn,
                    MRBID = s.MRBID,
                    Disposition = s.Disposition,
                    s.WarehouseName,
                    s.FlagRequestID,
                    s.RequestStatus
                })
                .Where(w => w.FlagRequestID == id)
                .ToList()
                .FirstOrDefault();

            var _requestUnits = uow.ECD_WarehouseDemixes.GetAll()
                .Where(w => w.FlagRequestID == id)
                .GroupBy(x => new { LotNumber = x.OriginalLotNumber, BoxNumber = x.OriginalBoxNumber, UnmergeLotNumber = x.DestinationLotNumber })
                .Select(s => new
                {
                    Count = s.Count().ToString(),
                    UnmergeLotNumber = s.Key.UnmergeLotNumber ?? string.Empty,
                    LotNumber = s.Key.LotNumber,
                    BoxNumber = s.Key.BoxNumber,
                    FlagRequestID = s.FirstOrDefault().FlagRequestID
                });

            ViewRequestViewModel _model = new ViewRequestViewModel();

            if (_request == null)
            {
                _model.InvalidID = true;
            }
            else
            {
                var _boxRequestStatuses = uow.BoxRequestStatuses.GetAll().IncludeMultiple(x => x.RequestStatus)
                    .Where(x => x.FlagRequestID == id).ToList();
                _model.CreatedBy = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).FullName;
                _model.CreatedOn = _request.CreatedOn;
                _model.MRBID = _request.MRBID;
                _model.Disposition = _request.Disposition.DispositionName;
                _model.Warehouse = _request.WarehouseName;
                _model.RequestID = id;
                _model.BoxCount = _requestUnits.Select(x => x.BoxNumber).Distinct().Count().ToString();
                _model.RequestStatus = _request.RequestStatus.RequestStatusName;
                List<FlaggedUnitLineItem> ungroupedList = _requestUnits.Select(x => new FlaggedUnitLineItem
                {
                    VisualIDCount = x.Count,
                    UnmergeLotID = x.UnmergeLotNumber,
                    BoxID = x.BoxNumber,
                    LotID = x.LotNumber,
                    FlagRequestID = x.FlagRequestID
                }).ToList();
                ungroupedList.ForEach(x => x.BoxRequestStatus = _boxRequestStatuses.FirstOrDefault(z => z.BoxNumber == x.BoxID).RequestStatus.RequestStatusName);
                var _totalBoxes = ungroupedList.Count;
                var _numerator = (double)(_totalBoxes - ungroupedList.Where(x => x.BoxRequestStatus == "New Request" || x.BoxRequestStatus == "In Process").Count());
                _model.PercentDone = _totalBoxes == 0 ? "0" : Math.Round(((_numerator / _totalBoxes) * 100), 0).ToString();

                var count = 0;
                List<FlaggedUnitLineItem> tempUnits = new List<FlaggedUnitLineItem>();
                ungroupedList.ForEach(x => {
                    if(count < 5)
                    {
                        tempUnits.Add(x);
                        count++;
                    }
                    if(count == 5)
                    {
                        _model.FlaggedUnits.Add(new Tuple<string, List<FlaggedUnitLineItem>>(
                            String.Join("V<3:)", tempUnits.Select(a => a.BoxID).ToList()),
                            new List<FlaggedUnitLineItem>(tempUnits)));
                        count = 0;
                        tempUnits.Clear();
                    }
                });
                if(tempUnits.Count > 0)
                {
                    _model.FlaggedUnits.Add(new Tuple<string, List<FlaggedUnitLineItem>>(
                            String.Join("V<3:)", tempUnits.Select(a => a.BoxID).ToList()),
                            new List<FlaggedUnitLineItem>(tempUnits)));
                }
            }

            return View(_model);
        }

        public bool StartProcessing(int id)
        {
            var _request = uow.FlagRequests.GetById(id);
            var _inProcessID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName == "In Process").RequestStatusID;
            var _newRequestID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName == "New Request").RequestStatusID;
            if (_request.RequestStatusID == _newRequestID)
            {
                _request.RequestStatusID = _inProcessID;
                _request.LastModified = DateTime.UtcNow;
                _request.LastModifiedBy = (User as ISEPrincipal).WUFTUser.IdSid;

                uow.FlagRequests.Update(_request);
                try
                {
                    uow.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;

        }

        public ActionResult ViewBarcodes(string boxIds, int flagRequestId)
        {
            List<string> boxes = boxIds.Split(new string[] { "V<3:)" },StringSplitOptions.None).Where(x=>!String.IsNullOrEmpty(x)).ToList();
            //int flagRequestId = uow.BoxRequestStatuses.GetAll().FirstOrDefault(x => x.BoxNumber == boxes.FirstOrDefault()).FlagRequestID;
            var _requestGroups = uow.ECD_WarehouseDemixes.GetAll()
                .IncludeMultiple(x => x.FlagRequest, x => x.FlagRequest.Disposition)
                .Where(w => w.FlagRequestID == flagRequestId && boxes.Contains(w.OriginalBoxNumber))
                .GroupBy(g => new
                {
                    MRBID = g.FlagRequest.MRBID,
                    g.OriginalBoxNumber,
                    g.OriginalLotNumber,
                    g.DestinationLotNumber,
                    DispositionName = g.FlagRequest.Disposition.DispositionName,
                    g.FlagRequest.CreatedBy,
                    g.FlagRequest.CreatedOn,
                    g.FlagRequest.WarehouseName
                })
                .Select(g => new
                {
                    DispositionName = g.Key.DispositionName,
                    CreatedBy = g.Key.CreatedBy,
                    CreatedOn = g.Key.CreatedOn,
                    Warehouse = g.Key.WarehouseName,
                    MRBID = g.Key.MRBID,
                    OriginalBoxNumber = g.Key.OriginalBoxNumber,
                    OriginalLotNumber = g.Key.OriginalLotNumber,
                    DestinationLotNumber = g.Key.DestinationLotNumber,
                    Quantity = g.Count()
                })
                .ToList();

            var _requestGroup = _requestGroups.FirstOrDefault();

            OperatorBarcodeViewModel _model = new OperatorBarcodeViewModel();

            var _boxRequestStatuses = uow.BoxRequestStatuses.GetAll().IncludeMultiple(x => x.RequestStatus)
                .Where(x => x.FlagRequestID == flagRequestId && boxIds.Contains(x.BoxNumber)).ToList();
            _model.MRBID = _requestGroup.MRBID;
            _model.Warehouse = _requestGroup.Warehouse;
            _model.Disposition = _requestGroup.DispositionName;
            _model.CreatedBy = _personHelper.GetPersonModelByIdsid(_requestGroup.CreatedBy).FullName;
            _model.CreatedOn = _requestGroup.CreatedOn;
            _model.Mode = _requestGroup.DispositionName.IndexOf("Scrap") >= 0 ? "Demix Mode" : "WLOT Mode";

            _model.FlaggedUnitGroups = _requestGroups.Select(x => new FlaggedUnitGroup
            {
                MRBID = x.MRBID,
                BoxID = x.OriginalBoxNumber,
                LotID = x.OriginalLotNumber,
                UnmergeLotID = x.DestinationLotNumber,
                Quantity = x.Quantity,
                Disposition = x.DispositionName,
                CreatedBy = _personHelper.GetPersonModelByIdsid(x.CreatedBy).FullName,
                CreatedOn = x.CreatedOn,
                Warehouse = x.Warehouse
            }).ToList();
            _model.FlaggedUnitGroups.ForEach(x => x.BoxRequestStatus = _boxRequestStatuses.FirstOrDefault(z => z.BoxNumber == x.BoxID).RequestStatus.RequestStatusName);

            StartProcessing(boxes, flagRequestId);

            return View(_model);
        }

        private void StartProcessing(List<string> boxIds, int flagRequestID)
        {
            var _request = uow.FlagRequests.GetById(flagRequestID);
            var _inProcessID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName == "In Process").RequestStatusID;
            var _newRequestID = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName == "New Request").RequestStatusID;

            if (_request.RequestStatusID == _newRequestID)
            {
                _request.RequestStatusID = _inProcessID;
                _request.LastModified = DateTime.UtcNow;
                _request.LastModifiedBy = (User as ISEPrincipal).WUFTUser.IdSid;

                uow.FlagRequests.Update(_request);
            }

            uow.BoxRequestStatuses.GetAll().Where(x => boxIds.Contains(x.BoxNumber)).ToList().ForEach(box => {
                if(box.RequestStatusID == _newRequestID)
                {
                    box.RequestStatusID = _inProcessID;
                    box.LastModifiedOn = DateTime.UtcNow;
                    box.LastModifiedBy = (User as ISEPrincipal).WUFTUser.IdSid;

                    uow.BoxRequestStatuses.Update(box);
                }
            });

            uow.SaveChanges();
        }
    }
}