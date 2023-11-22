using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WUFT.DATA;
using WUFT.EF;
using WUFT.NET.Util;
using WUFT.NET.ViewModels.Report;
using WUFT.NET.ViewModels.Shared;

namespace WUFT.NET.Controllers
{
    [ISESecurity(Roles = "AMR\\ISE Logistics_WUFT_Operator, AMR\\ISE Logistics_WUFT_Admin, AMR\\ISE Logistics_WUFT_QRE, GAR\\FlexAppsSupport")]
    public class ReportController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly PersonHelper _personHelper;
        public ReportController(IUnitOfWork uow)
        {
            this.uow = uow;
            _personHelper = new PersonHelper(uow);
        }
        // GET: Report
        //[ISESecurity(Roles = @"ISE Logistics_WUFT_QRE")]
        public ActionResult Index()
        {
            var _warehouses = new List<SelectListItem> { new SelectListItem() { Value = "0", Text = "Show All", Selected = true } };
            uow.Warehouses.GetAll().ToList().ForEach(x => _warehouses.Add(new SelectListItem() { Value = x.PlantCode, Text = x.PlantCode })); 

            var _returnModel = new ReportIndexParentViewModel();
            _returnModel.Warehouses = _warehouses;
            var _completedID = uow.RequestStatuses.GetAll().ToList();
            var _twoWeeksAgo = DateTime.UtcNow.AddDays(-14);

            //changes made for FS-INC100004856

            var _displaydate = DateTime.UtcNow.AddDays(-90);  //vsharm6x
            List<ReportLineItemViewModel> _requests = new List<ReportLineItemViewModel>();

            for (int i = 0; i < _completedID.Count; i++)
            {
                if (_completedID[i].RequestStatusName == "In Process" || _completedID[i].RequestStatusName == "New Request")
                {

                    if (_completedID[i].RequestStatusName == "In Process")
                    {
                        var _r1 = uow.FlagRequests.GetAll()
                       .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                       .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "In Process"))
                       .Select(x => new ReportLineItemViewModel
                       {
                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                           BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                           UnitQty = x.FlaggedUnits.Count,
                           RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
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
                       .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "New Request"))
                       .Select(x => new ReportLineItemViewModel
                       {
                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                           BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                           UnitQty = x.FlaggedUnits.Count,
                           RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
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
                                   .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed") && !(x.CreatedOn < _displaydate))
                                   .Select(x => new ReportLineItemViewModel
                                   {
                                       LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                       BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                       UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                       UnitQty = x.FlaggedUnits.Count,
                                       RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                       Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
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
                                   .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Stopped with Units Missing") && !(x.CreatedOn < _displaydate))
                                   .Select(x => new ReportLineItemViewModel
                                   {
                                       LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                       BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                       UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                       UnitQty = x.FlaggedUnits.Count,
                                       RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                       Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
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
                                                          .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed with Units Missing") && !(x.CreatedOn < _displaydate))
                                                          .Select(x => new ReportLineItemViewModel
                                                          {
                                                              LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                                              BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                                              UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                                              UnitQty = x.FlaggedUnits.Count,
                                                              RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                                              Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
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

            // var _requests = uow.FlagRequests.GetAll()
            //.Where(x => x.FlaggedUnits.Count > 0 && !(x.RequestStatus.RequestStatusName == "Complete" && x.LastModified > _twoWeeksAgo) && x.DispositionID != 9)
            //.IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition, x => x.RequestStatus)
            //.Select(x => new ReportLineItemViewModel
            //{
            //    LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
            //    BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
            //    UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
            //    UnitQty = x.FlaggedUnits.Count,
            //    RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
            //    Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
            //    LastModified = x.LastModified,
            //    CreatedOn = x.CreatedOn,
            //    Requestor = x.CreatedBy,
            //    RequestID = x.FlagRequestID,
            //    MRBID = x.MRBID,
            //    Warehouse = x.WarehouseName
            //}).ToList();
            
            _requests.ForEach(x => x.Requestor = _personHelper.GetPersonModelByIdsid(x.Requestor).FullName);
            // _returnModel.Requests = new ReportRequestList { Requests = _requests }; //vsharm6x
            _returnModel.Requests = new ReportRequestList { Requests = _requests.OrderByDescending(d => d.CreatedOn).ToList() }; //vsharm6x
            return View(_returnModel);
        }

        public ActionResult FilterRequests(string _warehouse = "", string _startDate = "", string _endDate = "")
        {
            DateTime _startDateTime;
            DateTime _endDateTime;

            DateTime.TryParse(_startDate, out _startDateTime);
            DateTime.TryParse(_endDate, out _endDateTime);
            _endDateTime = _endDateTime == DateTime.MinValue ? DateTime.MaxValue : _endDateTime;
            TimeSpan ts = new TimeSpan(23, 59, 59);
            _endDateTime = _endDateTime.Date + ts;

            var _requests = uow.FlagRequests.GetAll()
                .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                .Where(x =>
                    (x.FlaggedUnits.Count > 0) &&
                    (String.IsNullOrEmpty(_warehouse) || _warehouse == "0" || _warehouse == x.WarehouseName) &&
                    (_startDateTime <= x.CreatedOn) &&
                    (_endDateTime >= x.CreatedOn)
                 )
                .Select(x => new ReportLineItemViewModel
                {
                    LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                    BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                    UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                    UnitQty = x.FlaggedUnits.Count,
                    RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                    Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                    LastModified = x.LastModified,
                    CreatedOn = x.CreatedOn,
                    Requestor = x.CreatedBy,
                    RequestID = x.FlagRequestID,
                    MRBID = x.MRBID,
                    Warehouse = x.WarehouseName
                }).ToList();

            return PartialView("_RequestTable", _requests);
        }

        public ActionResult UpdateRequest(ReportIndexParentViewModel model)
        {
            var _warehouses = new List<SelectListItem> { new SelectListItem() { Value = "0", Text = "Show All", Selected = true } };
            uow.Warehouses.GetAll().ToList().ForEach(x => _warehouses.Add(new SelectListItem() { Value = x.PlantCode, Text = x.PlantCode })); 

            List<ReportLineItemViewModel> _requests = new List<ReportLineItemViewModel>();
            var _returnModel = new ReportIndexParentViewModel();
            _returnModel.Warehouses = _warehouses;
            var _completedID = uow.RequestStatuses.GetAll().ToList();

            if (model.SelectedWarehouse == "0")
            {
                for (int i = 0; i < _completedID.Count; i++)
                {
                    if (_completedID[i].RequestStatusName == "In Process" || _completedID[i].RequestStatusName == "New Request")
                    {

                        if (_completedID[i].RequestStatusName == "In Process")
                        {
                            var _r1 = uow.FlagRequests.GetAll()
                           .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                           .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "In Process") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd))
                           .Select(x => new ReportLineItemViewModel
                           {
                               LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                               BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                               UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                               UnitQty = x.FlaggedUnits.Count,
                               RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                               Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                               LastModified = x.LastModified,
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
                           .Select(x => new ReportLineItemViewModel
                           {
                               LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                               BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                               UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                               UnitQty = x.FlaggedUnits.Count,
                               RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                               Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                               LastModified = x.LastModified,
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
                                       .Select(x => new ReportLineItemViewModel
                                       {
                                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                           BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                           UnitQty = x.FlaggedUnits.Count,
                                           RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                           LastModified = x.LastModified,
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
                                       .Select(x => new ReportLineItemViewModel
                                       {
                                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                           BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                           UnitQty = x.FlaggedUnits.Count,
                                           RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                           LastModified = x.LastModified,
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
                                                              .Select(x => new ReportLineItemViewModel
                                                              {
                                                                  LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                                                  BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                                                  UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                                                  UnitQty = x.FlaggedUnits.Count,
                                                                  RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                                                  Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                                                  LastModified = x.LastModified,
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
            }

            else 
            {
                for (int i = 0; i < _completedID.Count; i++)
                {
                    if (_completedID[i].RequestStatusName == "In Process" || _completedID[i].RequestStatusName == "New Request")
                    {

                        if (_completedID[i].RequestStatusName == "In Process")
                        {
                            var _r1 = uow.FlagRequests.GetAll()
                           .IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition)
                           .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "In Process") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd) && (x.WarehouseName == model.SelectedWarehouse))
                           .Select(x => new ReportLineItemViewModel
                           {
                               LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                               BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                               UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                               UnitQty = x.FlaggedUnits.Count,
                               RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                               Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                               LastModified = x.LastModified,
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
                           .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "New Request") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd) && (x.WarehouseName == model.SelectedWarehouse))
                           .Select(x => new ReportLineItemViewModel
                           {
                               LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                               BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                               UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                               UnitQty = x.FlaggedUnits.Count,
                               RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                               Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                               LastModified = x.LastModified,
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
                                       .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd) && (x.WarehouseName == model.SelectedWarehouse))
                                       .Select(x => new ReportLineItemViewModel
                                       {
                                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                           BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                           UnitQty = x.FlaggedUnits.Count,
                                           RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                           LastModified = x.LastModified,
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
                                       .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Stopped with Units Missing") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd) && (x.WarehouseName == model.SelectedWarehouse))
                                       .Select(x => new ReportLineItemViewModel
                                       {
                                           LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                           BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                           UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                           UnitQty = x.FlaggedUnits.Count,
                                           RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                           Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                           LastModified = x.LastModified,
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
                                                              .Where(x => x.FlaggedUnits.Count > 0 && (x.RequestStatus.RequestStatusName == "Completed with Units Missing") && (x.CreatedOn >= model.TimePeriodStart && x.CreatedOn <= model.TimePeriodEnd) && (x.WarehouseName == model.SelectedWarehouse))
                                                              .Select(x => new ReportLineItemViewModel
                                                              {
                                                                  LotIDs = x.FlaggedUnits.Select(y => y.OriginalLotNumber).Distinct().ToList(),
                                                                  BoxIDs = x.FlaggedUnits.Select(y => y.OriginalBoxNumber).Distinct().ToList(),
                                                                  UnmergeLotIDs = x.FlaggedUnits.Select(y => y.DestinationLotNumber).Distinct().ToList(),
                                                                  UnitQty = x.FlaggedUnits.Count,
                                                                  RequestStatus = x.RequestStatus.RequestStatusName.ToString(),
                                                                  Disposition = "(" + x.Disposition.DispositionID + ") " + x.Disposition.DispositionName,
                                                                  LastModified = x.LastModified,
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
            }
            
            //_returnModel.Requests = new ReportRequestList { Requests = _requests }; //vsharm6x
            _returnModel.Requests = new ReportRequestList { Requests = _requests.OrderByDescending(d => d.CreatedOn).ToList() }; //vsharm6x

            return View("Index", ( _returnModel )); 

        }
    }
}