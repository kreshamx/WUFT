using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WUFT.DATA;
using WUFT.NET.Util;
using WUFT.EF;
using WUFT.NET.ViewModels.Shared;
using WUFT.NET.ViewModels.Admin;

namespace WUFT.NET.Controllers
{
    [ISESecurity(Roles = "AMR\\ISE Logistics_WUFT_Admin, AMR\\ISE Logistics_ISE_Developer")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly PersonHelper _personHelper;

        public AdminController(IUnitOfWork uow)
        {
            this.uow = uow;
            _personHelper = new PersonHelper(uow);
        }

        [HttpPost]
        public ActionResult UpdateRequests(RequestIndexParentViewModel model)
        {
            //model.Requests.Where(x => !x.OriginalRequestStatusID.ToString().Equals(x.SelectedStatus)).ToList().ForEach(x =>
            //{
            //    var oldRequest = uow.FlagRequests.GetAll().FirstOrDefault(y => y.FlagRequestID == x.RequestID);
            //    oldRequest.RequestStatusID = Int32.Parse(x.SelectedStatus);
            //    oldRequest.LastModified = DateTime.UtcNow;
            //    oldRequest.LastModifiedBy = User != null ? (User as ISEPrincipal).WUFTUser.IdSid : "Admin";
            //    uow.FlagRequests.Update(oldRequest);
            //});

            //uow.SaveChanges();

            //return RedirectToAction("Index");

            //Changes done by rshanm3x

            var _statusList = new List<SelectListItem>();
            var _displaydate = DateTime.UtcNow.AddDays(-90); //vsharm6x
            uow.RequestStatuses.GetAll().ToList().ForEach(x => _statusList.Add(new SelectListItem() { Value = x.RequestStatusID.ToString(), Text = x.RequestStatusName }));
            var _completedID = uow.RequestStatuses.GetAll().ToList();

            //changes made for FS-INC100004856

            List<RequestLineItemViewModel> _requests = new List<RequestLineItemViewModel>();

            for (int i = 0; i < _completedID.Count; i++)
            {
                if (_completedID[i].RequestStatusName == "In Process" || _completedID[i].RequestStatusName == "New Request")
                {

                    if (_completedID[i].RequestStatusName == "In Process")
                    {
                        var _r1 = uow.FlagRequests.GetAll()
                       .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                       .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "In Process") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                       .Select(x => new RequestLineItemViewModel
                       {
                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                           TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                           UnitQty = x.FlaggedUnits.Count,
                           OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                           OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                           SelectedStatus = x.RequestStatus.RequestStatusName,
                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                           CreatedOn = x.CreatedOn,
                           Requestor = x.CreatedBy,
                           RequestID = x.FlagRequestID,
                           MRBID = x.MRBID,
                           Warehouse = x.WarehouseName
                       }).ToList();
                        _requests.AddRange(_r1);

                    }
                    else
                    {
                        var _r2 = uow.FlagRequests.GetAll()
                       .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                       .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "New Request") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                       .Select(x => new RequestLineItemViewModel
                       {
                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                           TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                           UnitQty = x.FlaggedUnits.Count,
                           OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                           OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                           SelectedStatus = x.RequestStatus.RequestStatusName,
                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                           CreatedOn = x.CreatedOn,
                           Requestor = x.CreatedBy,
                           RequestID = x.FlagRequestID,
                           MRBID = x.MRBID,
                           Warehouse = x.WarehouseName
                       }).ToList();
                        _requests.AddRange(_r2);
                    }

                }
                else if (_completedID[i].RequestStatusName == "Completed" || _completedID[i].RequestStatusName == "Stopped with Units Missing" || _completedID[i].RequestStatusName == "Completed with Units Missing")
                {
                    if (_completedID[i].RequestStatusName == "Completed")
                    {
                        var _r3 = uow.FlagRequests.GetAll()
                                   .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                                   .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                                   .Select(x => new RequestLineItemViewModel
                                   {
                                       LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                       TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                       UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                       UnitQty = x.FlaggedUnits.Count,
                                       OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                       OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                                       SelectedStatus = x.RequestStatus.RequestStatusName,
                                       Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                       CreatedOn = x.CreatedOn,
                                       Requestor = x.CreatedBy,
                                       RequestID = x.FlagRequestID,
                                       MRBID = x.MRBID,
                                       Warehouse = x.WarehouseName
                                   }).ToList();
                        _requests.AddRange(_r3);
                    }
                    else if (_completedID[i].RequestStatusName == "Stopped with Units Missing")
                    {
                        var _r4 = uow.FlagRequests.GetAll()
                                   .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                                   .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Stopped with Units Missing") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                                   .Select(x => new RequestLineItemViewModel
                                   {
                                       LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                       TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                       UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                       UnitQty = x.FlaggedUnits.Count,
                                       OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                       OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                                       SelectedStatus = x.RequestStatus.RequestStatusName,
                                       Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                       CreatedOn = x.CreatedOn,
                                       Requestor = x.CreatedBy,
                                       RequestID = x.FlagRequestID,
                                       MRBID = x.MRBID,
                                       Warehouse = x.WarehouseName
                                   }).ToList();
                        _requests.AddRange(_r4);
                    }
                    else
                    {
                        var _r5 = uow.FlagRequests.GetAll()
                                                          .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                                                          .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed with Units Missing") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                                                          .Select(x => new RequestLineItemViewModel
                                                          {
                                                              LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                                              TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                                              UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                                              UnitQty = x.FlaggedUnits.Count,
                                                              OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                                              OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                                                              SelectedStatus = x.RequestStatus.RequestStatusName,
                                                              Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                                              CreatedOn = x.CreatedOn,
                                                              Requestor = x.CreatedBy,
                                                              RequestID = x.FlagRequestID,
                                                              MRBID = x.MRBID,
                                                              Warehouse = x.WarehouseName
                                                          }).ToList();
                        _requests.AddRange(_r5);
                    }

                }

            }

            List<string> allBoxes = _requests.SelectMany(x => x.TempBoxIDs).ToList();
            List<string> _duplicateBoxes = allBoxes.GroupBy(x => x).ToList().Where(x => x.Count() > 1).ToList().Select(x => x.Key).ToList();

            _requests.ForEach(x =>
            {
                x.Requestor = _personHelper.GetPersonModelByIdsid(x.Requestor).FullName;
                x.Statuses = _statusList;
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

            return View("Index", new RequestIndexParentViewModel { Requests = _requests });
        }

        [HttpPost]
        public ActionResult UpdateRequest(ViewRequestViewModel model, string btnReFlag)
        {
            var _request = uow.FlagRequests.GetAll().FirstOrDefault(x => x.FlagRequestID == model.RequestID);

            //vsharm6x : Re-flag boxes
            if(string.IsNullOrEmpty(btnReFlag))
            {
                //update request status if it changed
                if (_request.RequestStatusID != Int32.Parse(model.RequestStatusID))
                {
                    _request.RequestStatusID = Int32.Parse(model.RequestStatusID);
                    _request.LastModified = DateTime.UtcNow;
                    _request.LastModifiedBy = User != null ? (User as ISEPrincipal).WUFTUser.IdSid : "Admin";
                    uow.FlagRequests.Update(_request);
                }

                foreach (var box in model.FlaggedBoxes)
                {
                    var boxToUpdate = uow.BoxRequestStatuses.GetAll().FirstOrDefault(x => x.BoxNumber == box.GoodBoxID && x.FlagRequestID == model.RequestID);
                    if (box.BoxRequestStatus != boxToUpdate.RequestStatusID.ToString())
                    {
                        boxToUpdate.RequestStatusID = Int32.Parse(box.BoxRequestStatus);
                        boxToUpdate.LastModifiedOn = DateTime.UtcNow;
                        boxToUpdate.LastModifiedBy = User != null ? (User as ISEPrincipal).WUFTUser.IdSid : "Admin";
                        uow.BoxRequestStatuses.Update(boxToUpdate);
                    }
                }
            }
            else
            {
                _request.RequestStatusID = Int32.Parse(model.RequestStatusID);
                _request.LastModified = DateTime.UtcNow;
                _request.LastModifiedBy = User != null ? (User as ISEPrincipal).WUFTUser.IdSid : "Admin";
                _request.DispositionID = 9;
                uow.FlagRequests.Update(_request);
            }
           

            uow.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ViewRequest(int id)
        {
            var _statusList = new List<SelectListItem>();
            uow.RequestStatuses.GetAll().ToList().ForEach(x => _statusList.Add(new SelectListItem() { Value = x.RequestStatusID.ToString(), Text = x.RequestStatusName }));

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
                var _boxRequestStatuses = uow.BoxRequestStatuses.GetAll().IncludeMultiple(x => x.RequestStatus).Where(x => x.FlagRequestID == id).ToList();
                _model.CreatedBy = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).FullName;
                _model.CreatedByEmail = _personHelper.GetPersonModelByIdsid(_request.CreatedBy).Email;
                _model.LastModifiedOn = _request.LastModified;
                _model.MRBID = _request.MRBID;
                _model.Disposition = _request.Disposition.DispositionName;
                _model.Warehouse = _request.WarehouseName;
                _model.RequestID = id;
                _model.RequestStatusID = _request.RequestStatus.RequestStatusID.ToString();
                _model.Statuses = _statusList;
                _model.BoxCount = _boxRequestStatuses.Count.ToString();

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
                }).ToList();

                _model.FlaggedBoxes.ForEach(x =>
                {
                    x.BoxRequestStatus = _boxRequestStatuses.FirstOrDefault(z => z.BoxNumber == x.GoodBoxID).RequestStatus.RequestStatusID.ToString();
                    x.Statuses = _statusList;
                    x.Statuses.FirstOrDefault(s => s.Value == x.BoxRequestStatus).Selected = true;
                });
            }

            return View(_model);
        }

        public ActionResult Index()
        {
            var _statusList = new List<SelectListItem>();
            uow.RequestStatuses.GetAll().ToList().ForEach(x => _statusList.Add(new SelectListItem() { Value = x.RequestStatusID.ToString(), Text = x.RequestStatusName }));
            var _completedID = uow.RequestStatuses.GetAll().ToList();
            var _twoWeeksAgo = DateTime.UtcNow.AddDays(-14);

            //changes made for FS-INC100004856

            var _displaydate = DateTime.UtcNow.AddDays(-90);  //vsharm6x
            List<RequestLineItemViewModel> _requests = new List<RequestLineItemViewModel>();

            for (int i = 0; i < _completedID.Count; i++)
            {
                if (_completedID[i].RequestStatusName == "In Process" || _completedID[i].RequestStatusName == "New Request")
                {

                    if (_completedID[i].RequestStatusName == "In Process")
                    {
                        var _r1 = uow.FlagRequests.GetAll()
                       .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                       .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "In Process") && !(x.CreatedOn < _displaydate))
                       .Select(x => new RequestLineItemViewModel
                       {
                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                           TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                           UnitQty = x.FlaggedUnits.Count,
                           OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                           OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                           SelectedStatus = x.RequestStatus.RequestStatusName,
                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                           CreatedOn = x.CreatedOn,
                           Requestor = x.CreatedBy,
                           RequestID = x.FlagRequestID,
                           MRBID = x.MRBID,
                           Warehouse = x.WarehouseName
                       }).ToList();
                        _requests.AddRange(_r1);

                    }
                    else
                    {
                        var _r1 = uow.FlagRequests.GetAll()
                       .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                       .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "New Request") && !(x.CreatedOn < _displaydate))
                       .Select(x => new RequestLineItemViewModel
                       {
                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                           TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                           UnitQty = x.FlaggedUnits.Count,
                           OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                           OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                           SelectedStatus = x.RequestStatus.RequestStatusName,
                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                           CreatedOn = x.CreatedOn,
                           Requestor = x.CreatedBy,
                           RequestID = x.FlagRequestID,
                           MRBID = x.MRBID,
                           Warehouse = x.WarehouseName
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
                                   .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed") && !(x.CreatedOn < _displaydate))
                                   .Select(x => new RequestLineItemViewModel
                                   {
                                       LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                       TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                       UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                       UnitQty = x.FlaggedUnits.Count,
                                       OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                       OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                                       SelectedStatus = x.RequestStatus.RequestStatusName,
                                       Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                       CreatedOn = x.CreatedOn,
                                       Requestor = x.CreatedBy,
                                       RequestID = x.FlagRequestID,
                                       MRBID = x.MRBID,
                                       Warehouse = x.WarehouseName
                                   }).ToList();
                        _requests.AddRange(_r2);
                    }
                    else if (_completedID[i].RequestStatusName == "Stopped with Units Missing")
                    {
                        var _r2 = uow.FlagRequests.GetAll()
                                   .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                                   .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Stopped with Units Missing") && !(x.CreatedOn < _displaydate))
                                   .Select(x => new RequestLineItemViewModel
                                   {
                                       LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                       TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                       UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                       UnitQty = x.FlaggedUnits.Count,
                                       OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                       OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                                       SelectedStatus = x.RequestStatus.RequestStatusName,
                                       Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                       CreatedOn = x.CreatedOn,
                                       Requestor = x.CreatedBy,
                                       RequestID = x.FlagRequestID,
                                       MRBID = x.MRBID,
                                       Warehouse = x.WarehouseName
                                   }).ToList();
                        _requests.AddRange(_r2);
                    }
                    else
                    {
                        var _r2 = uow.FlagRequests.GetAll()
                                                          .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                                                          .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed with Units Missing") && !(x.CreatedOn < _displaydate))
                                                          .Select(x => new RequestLineItemViewModel
                                                          {
                                                              LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                                              TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                                              UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                                              UnitQty = x.FlaggedUnits.Count,
                                                              OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                                              OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                                                              SelectedStatus = x.RequestStatus.RequestStatusName,
                                                              Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                                              CreatedOn = x.CreatedOn,
                                                              Requestor = x.CreatedBy,
                                                              RequestID = x.FlagRequestID,
                                                              MRBID = x.MRBID,
                                                              Warehouse = x.WarehouseName
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
            //        OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
            //        SelectedStatus = x.RequestStatus.RequestStatusName,
            //        Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
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
                x.Statuses = _statusList;
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

           
            var _returnModel = new RequestIndexParentViewModel();
            //_returnModel.Requests = _requests; //vsharm6x
            _returnModel.Requests = _requests.OrderByDescending(d => d.CreatedOn).ToList(); //vsharm6x

            return View(_returnModel);
        }

        public ActionResult UpdateRequest(RequestIndexParentViewModel model)
        {
            var _statusList = new List<SelectListItem>();
            uow.RequestStatuses.GetAll().ToList().ForEach(x => _statusList.Add(new SelectListItem() { Value = x.RequestStatusID.ToString(), Text = x.RequestStatusName }));
            var _completedID = uow.RequestStatuses.GetAll().ToList();
            var _twoWeeksAgo = DateTime.UtcNow.AddDays(-14);

            //ViewBag.TimePeriodStart = model.TimePeriodStart;
            //ViewBag.TimePeriodEnd = model.TimePeriodEnd;

            //changes made for FS-INC100004856

            var _displaydate = DateTime.UtcNow.AddDays(-90);   //vsharm6x
            List<RequestLineItemViewModel> _requests = new List<RequestLineItemViewModel>();

            for (int i = 0; i < _completedID.Count; i++)
            {
                if (_completedID[i].RequestStatusName == "In Process" || _completedID[i].RequestStatusName == "New Request")
                {

                    if (_completedID[i].RequestStatusName == "In Process")
                    {
                        var _r1 = uow.FlagRequests.GetAll()
                       .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                       .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "In Process") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                       .Select(x => new RequestLineItemViewModel
                       {
                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                           TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                           UnitQty = x.FlaggedUnits.Count,
                           OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                           OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                           SelectedStatus = x.RequestStatus.RequestStatusName,
                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                           CreatedOn = x.CreatedOn,
                           Requestor = x.CreatedBy,
                           RequestID = x.FlagRequestID,
                           MRBID = x.MRBID,
                           Warehouse = x.WarehouseName
                       }).ToList();
                        _requests.AddRange(_r1);

                    }
                    else
                    {
                        var _r1 = uow.FlagRequests.GetAll()
                       .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                       .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "New Request") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                       .Select(x => new RequestLineItemViewModel
                       {
                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                           TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                           UnitQty = x.FlaggedUnits.Count,
                           OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                           OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                           SelectedStatus = x.RequestStatus.RequestStatusName,
                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                           CreatedOn = x.CreatedOn,
                           Requestor = x.CreatedBy,
                           RequestID = x.FlagRequestID,
                           MRBID = x.MRBID,
                           Warehouse = x.WarehouseName
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
                                   .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                                   .Select(x => new RequestLineItemViewModel
                                   {
                                       LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                       TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                       UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                       UnitQty = x.FlaggedUnits.Count,
                                       OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                       OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                                       SelectedStatus = x.RequestStatus.RequestStatusName,
                                       Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                       CreatedOn = x.CreatedOn,
                                       Requestor = x.CreatedBy,
                                       RequestID = x.FlagRequestID,
                                       MRBID = x.MRBID,
                                       Warehouse = x.WarehouseName
                                   }).ToList();
                        _requests.AddRange(_r2);
                    }
                    else if (_completedID[i].RequestStatusName == "Stopped with Units Missing")
                    {
                        var _r2 = uow.FlagRequests.GetAll()
                                   .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                                   .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Stopped with Units Missing") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                                   .Select(x => new RequestLineItemViewModel
                                   {
                                       LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                       TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                       UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                       UnitQty = x.FlaggedUnits.Count,
                                       OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                       OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                                       SelectedStatus = x.RequestStatus.RequestStatusName,
                                       Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                       CreatedOn = x.CreatedOn,
                                       Requestor = x.CreatedBy,
                                       RequestID = x.FlagRequestID,
                                       MRBID = x.MRBID,
                                       Warehouse = x.WarehouseName
                                   }).ToList();
                        _requests.AddRange(_r2);
                    }
                    else
                    {
                        var _r2 = uow.FlagRequests.GetAll()
                                                          .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                                                          .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed with Units Missing") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                                                          .Select(x => new RequestLineItemViewModel
                                                          {
                                                              LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                                              TempBoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                                              UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                                              UnitQty = x.FlaggedUnits.Count,
                                                              OriginalRequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                                              OriginalRequestStatusID = x.RequestStatus.RequestStatusID,
                                                              SelectedStatus = x.RequestStatus.RequestStatusName,
                                                              Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                                              CreatedOn = x.CreatedOn,
                                                              Requestor = x.CreatedBy,
                                                              RequestID = x.FlagRequestID,
                                                              MRBID = x.MRBID,
                                                              Warehouse = x.WarehouseName
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
                x.Statuses = _statusList;
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

            //var _returnModel = new RequestIndexParentViewModel();
            //_returnModel.Requests = _requests;

            //return View("Index", new RequestIndexParentViewModel { Requests = _requests }); //vsharm6x
            return View("Index", new RequestIndexParentViewModel { Requests = _requests.OrderByDescending(d => d.CreatedOn).ToList()}); //vsharm6x
        }
    }
}