using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.Mvc;
using WUFT.DATA;
using WUFT.EF;
using WUFT.NET.Util;
using WUFT.NET.ViewModels.Report;
using WUFT.NET.ViewModels.Shared;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;
using System.Data.SqlClient;
using System.Data.Common;
using Glimpse.Core.ClientScript;
using Microsoft.Office.Interop.Excel;
using System.Web.UI.WebControls;
using System.Configuration;

namespace WUFT.NET.Controllers
{
    [Class1]
    public class FileController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly PersonHelper _personHelper;

        public FileController(IUnitOfWork uow)
        {
            this.uow = uow;
            _personHelper = new PersonHelper(uow);
        }

        //Modified by rshanm3x for FS-INC100003465
        public FileResult ExportList(RequestIndexParentViewModel model, string id, string btnRep)
        {
            var idSplit = model.IdList.Split(new string[] { "V<3:)" }, StringSplitOptions.None);
            List<int> ids = new List<int>();
            idSplit.ToList().ForEach(x => { ids.Add(Int32.Parse(x)); });

            if (btnRep == "")
            {
                byte[] fileBytes = GenerateExportFile(ids);
                string fileName = "WUFT Report " + DateTime.UtcNow.ToString(@"yyyy-MM-dd") + ".csv";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                byte[] fileBytes = GenerateExportFile1(ids);
                string fileName = "Box Level Summary Report " + DateTime.UtcNow.ToString(@"yyyy-MM-dd") + ".csv";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

        }

        public FileResult ExportQREViewRequest(WUFT.NET.ViewModels.QRE.ViewRequestViewModel model)
        {
            byte[] fileBytes = GenerateExportFile(new List<int>() { model.RequestID });
            string fileName = "WUFT Request " + DateTime.UtcNow.ToString(@"yyyy-MM-dd") + ".csv";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpPost]
        public ActionResult ExportReport(ReportIndexParentViewModel model, string btnRep) //vsharm6x : Box level summary report in Report View
        {
            if (model.IDs == null)
            {
                return RedirectToAction("UpdateRequest", "Report", model);
            }
            else if (btnRep == "")
            {
                List<int> intIds = model.IDs.Split(new string[] { "V<3:)" }, StringSplitOptions.None).Select(x => Int32.Parse(x)).ToList();
                byte[] fileBytes = GenerateExportFile(intIds);
                string fileName = "WUFT Request " + DateTime.UtcNow.ToString(@"yyyy-MM-dd") + ".csv";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            //TASK7720609 : Box level summary report, added by vsharm6x
            else
            {
                List<int> intIds = model.IDs.Split(new string[] { "V<3:)" }, StringSplitOptions.None).Select(x => Int32.Parse(x)).ToList();
                byte[] fileBytes = GenerateExportFile1(intIds);
                string fileName = "Box Level Summary Report " + DateTime.UtcNow.ToString(@"yyyy-MM-dd") + ".csv";
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
        }

        [Route("File/ExportSingleRequest/{id}")]
        public FileResult ExportSingleRequest(int id)
        {
            byte[] fileBytes = GenerateExportFile(new List<int>() { id });
            string fileName = "WUFT Request " + DateTime.UtcNow.ToString(@"yyyy-MM-dd") + ".csv";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public byte[] GenerateExportFile(List<int> ids)
        {
            //var boxStatuses = uow.BoxRequestStatuses.GetAll().IncludeMultiple(x => x.RequestStatus).Where(x => x.FlagRequestID == 46);
            var boxStatuses = uow.BoxRequestStatuses.GetAll().IncludeMultiple(x => x.RequestStatus).Where(x => ids.Contains(x.FlagRequestID));
            var stoppedWithUnitsMissing = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName.Equals("Stopped with Units Missing")).RequestStatusID;
            StringBuilder builder = new StringBuilder();
            builder.Append(String.Join(",", new string[] {
                "MRB Number",
                "LOT ID",
                "OLD BOX ID",
                "VISUAL ID",
                "UNIT STATUS",
                "FPO LOT ID",
                "NEW BOX ID",
                "DISPOSITION",
                "WAREHOUSE",
                "REQUESTOR",
                "REQUEST DATE",
                "REQUEST COMPLETE DATE",
                "REQUEST STATUS",
                "BOX STATUS",
                "BOX START DT",
                "BOX END DT",
                "TIME ELAPSED (hh:mm:ss)",
                "\n"
            }));

            StringBuilder sb = new StringBuilder();
            foreach (int i in ids)
            {
                sb.Append(i.ToString()).Append(",");
            }

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["WUFTDbContext"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand("Usp_GetExcelReportMultiVal", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@RequestID", sb.ToString().Trim()));
            //cmd.Parameters.Add(new SqlParameter("@RequestID", ids[0]));
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = ds.Tables[0];
            conn.Close();

            if (dt.Rows.Count > 0)
            {
                for(int i=0; i<dt.Rows.Count; i++)
                {
                    string RS = string.Empty;
                    if (dt.Rows[i]["REQUEST STATUS"].ToString() == "Stopped with Units Missing")
                    {
                        if(Convert.ToInt32(dt.Rows[i]["UnitFoundButRequestStopped"]) == 0)
                        {
                            RS = "NOT FOUND";
                        }
                        else
                        {
                            RS = "LOCATED";
                        }
                    }
                    else
                    {
                        if(Convert.ToInt32(dt.Rows[i]["UnitFound"]) == 0)
                        {
                            RS = "FOUND";
                        }
                        else
                        {
                            RS = "NOT FOUND";
                        }
                    }
                    builder.Append(string.Join(",", new string[]
                    {
                        dt.Rows[i]["MRB Number"].ToString(),
                        dt.Rows[i]["LOT ID"].ToString(),
                        dt.Rows[i]["OLD BOX ID"].ToString(),
                        dt.Rows[i]["VISUAL ID"].ToString(),
                        RS,
                        dt.Rows[i]["FPO LOT ID"].ToString(),
                        dt.Rows[i]["NEW BOX ID"].ToString(),
                        dt.Rows[i]["DISPOSITION"].ToString(),
                        dt.Rows[i]["WAREHOUSE"].ToString(),
                        dt.Rows[i]["REQUESTOR"].ToString(),
                        dt.Rows[i]["REQUEST DATE"].ToString(),
                        dt.Rows[i]["REQUEST COMPLETE DATE"].ToString(),
                        dt.Rows[i]["REQUEST STATUS"].ToString(),
                        dt.Rows[i]["BOX STATUS"].ToString(),
                        dt.Rows[i]["BOX START DT"].ToString(),
                        dt.Rows[i]["BOX END DT"].ToString(),
                        "\n"
                    }));
                }

            }
            dt.Dispose();



            //uow.FlagRequests.GetAll().IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition, x => x.RequestStatus)
            //    .Where(x => x.FlaggedUnits.Count > 0 && ids.Contains(x.FlagRequestID)).ToList()
            //    .ForEach(x => x.FlaggedUnits
            //    .ForEach(y =>
            //    {
            //        ; var boxStatus = boxStatuses.FirstOrDefault(z => z.BoxNumber == y.OriginalBoxNumber && z.FlagRequestID == y.FlagRequestID);
            //        var start = boxStatus.StartTime;
            //        var end = boxStatus.EndTime;
            //        builder.Append(
            //       String.Join(",", new string[]
            //       {
            //        x.MRBID,
            //        y.OriginalLotNumber,
            //        y.OriginalBoxNumber,
            //        y.SubstrateVisualSID,
            //       boxStatus.RequestStatusID == stoppedWithUnitsMissing ? (y.UnitFoundButRequestStopped ? "Located" : "Not Found") : y.UnitFound ? "Found" : "Not Found",
            //        y.DestinationLotNumber,
            //        y.DestinationBoxNumber,
            //        x.Disposition.DispositionName,
            //        x.WarehouseName,
            //        _personHelper.GetPersonModelByIdsid(x.CreatedBy).FullName.Replace(',',(char)0),
            //        ClientTimeZoneHelper.ConvertToLocalTimeAndFormat(x.CreatedOn, "MM/dd/yyyy HH:mm:ss"),//x.CreatedOn.ToString("M/d/yyyy hh:mm:ss") + " UTC",
            //        x.CompletedOn == null ? "" : ClientTimeZoneHelper.ConvertToLocalTimeAndFormat((DateTime)x.CompletedOn, "MM/dd/yyyy HH:mm:ss"),
            //        x.RequestStatus.RequestStatusName,
            //        boxStatus.RequestStatus.RequestStatusName,
            //        start == null? string.Empty : ClientTimeZoneHelper.ConvertToLocalTimeAndFormat((DateTime)start, "MM/d/yyyy HH:mm:ss"),
            //        end == null? string.Empty : ClientTimeZoneHelper.ConvertToLocalTimeAndFormat((DateTime)end, "MM/d/yyyy HH:mm:ss"),
            //        start == null || end == null ? string.Empty : ((TimeSpan)(end-start)).ToString(@"hh\:mm\:ss"),
            //        "\n" }));
            //    }
            //    ));

            var str = builder.ToString();
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }

        //Created by rshanm3x for FS-INC100003465
        public byte[] GenerateExportFile1(List<int> ids)
        {
            var boxStatuses = uow.BoxRequestStatuses.GetAll().IncludeMultiple(x => x.RequestStatus).Where(x => ids.Contains(x.FlagRequestID));
            var stoppedWithUnitsMissing = uow.RequestStatuses.GetAll().FirstOrDefault(x => x.RequestStatusName.Equals("Stopped with Units Missing")).RequestStatusID;
            StringBuilder builder = new StringBuilder();
            System.Data.DataTable dt = new System.Data.DataTable();

            //FS-INC100005772 : Change of Box levele template order
            DataColumn REQUESTOR = new DataColumn("REQUESTOR");
            REQUESTOR.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(REQUESTOR);

            DataColumn MRB_Number = new DataColumn("MRB Number");
            MRB_Number.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(MRB_Number);

            DataColumn WAREHOUSE = new DataColumn("WAREHOUSE");
            WAREHOUSE.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(WAREHOUSE);

            DataColumn LOT_ID = new DataColumn("LOT ID");
            LOT_ID.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(LOT_ID);

            DataColumn OLD_BOX_ID = new DataColumn("OLD BOX ID");
            OLD_BOX_ID.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(OLD_BOX_ID);

            DataColumn ORIGINAL_QTY = new DataColumn("ORIGINAL QTY");
            ORIGINAL_QTY.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(ORIGINAL_QTY);

            DataColumn FLAGGED_UNITS = new DataColumn("FLAGGED UNITS QTY");
            FLAGGED_UNITS.DataType = System.Type.GetType("System.Int32");
            dt.Columns.Add(FLAGGED_UNITS);

            DataColumn NEW_BOX_ID = new DataColumn("NEW BOX ID");
            NEW_BOX_ID.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(NEW_BOX_ID);

            DataColumn REQUEST_DATE = new DataColumn("REQUEST DATE");
            REQUEST_DATE.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(REQUEST_DATE);

            DataColumn BOX_START_DT = new DataColumn("BOX START DT");
            BOX_START_DT.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(BOX_START_DT);

            DataColumn BOX_END_DT = new DataColumn("BOX END DT");
            BOX_END_DT.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(BOX_END_DT);

            DataColumn TIME_ELAPSED = new DataColumn("TIME ELAPSED (hh:mm:ss)");
            TIME_ELAPSED.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(TIME_ELAPSED);

            DataColumn REQUEST_COMPLETE_DATE = new DataColumn("MATERIAL MASTER NO.");
            REQUEST_COMPLETE_DATE.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(REQUEST_COMPLETE_DATE);

            DataColumn DISPOSITION = new DataColumn("DISPOSITION");
            DISPOSITION.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(DISPOSITION);

            DataColumn MM_NO = new DataColumn("REQUEST COMPLETE DATE");
            MM_NO.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(MM_NO);

            DataColumn REQUEST_STATUS = new DataColumn("REQUEST STATUS");
            REQUEST_STATUS.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(REQUEST_STATUS);

            DataColumn BOX_STATUS = new DataColumn("BOX STATUS");
            BOX_STATUS.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(BOX_STATUS);

            DataColumn FPO_LOT_ID = new DataColumn("FPO LOT ID");
            FPO_LOT_ID.DataType = System.Type.GetType("System.String");
            dt.Columns.Add(FPO_LOT_ID);

            builder.Append(String.Join(",", new string[] {
                "REQUESTOR",
                "MRB Number",
                "WAREHOUSE",
                "LOT ID",
                "OLD BOX ID",
                "ORIGINAL QTY",
                "FLAGGED UNITS QTY",
                "NEW BOX ID",
                "REQUEST DATE",
                "BOX START DT",
                "BOX END DT",
                "TIME ELAPSED (hh:mm:ss)",
                "MATERIAL MASTER NO.",
                "DISPOSITION",
                "REQUEST COMPLETE DATE",
                "REQUEST STATUS",
                "BOX STATUS",
                "FPO LOT ID",
                "\n"
            }));
            try
            {
                uow.FlagRequests.GetAll().IncludeMultiple(x => x.FlaggedUnits, x => x.Disposition, x => x.RequestStatus)
                    .Where(x => x.FlaggedUnits.Count > 0 && ids.Contains(x.FlagRequestID)).ToList()
                    .ForEach(x => x.FlaggedUnits
                    .ForEach(y =>
                    {
                        var boxStatus = boxStatuses.FirstOrDefault(z => z.BoxNumber == y.OriginalBoxNumber && z.FlagRequestID == y.FlagRequestID);
                        var start = boxStatus != null ? boxStatus.StartTime : null;
                        var end = boxStatus != null ? boxStatus.EndTime : null;
                        dt.Rows.Add(_personHelper.GetPersonModelByIdsid(x.CreatedBy).FullName.Replace(',', (char)0)
                                   , x.MRBID
                                   , x.WarehouseName
                                   , y.OriginalLotNumber
                                   , y.OriginalBoxNumber
                                   , y.OriginalQty
                                   , y.FlaggedUnitsQty
                                   , y.DestinationBoxNumber
                                   , ClientTimeZoneHelper.ConvertToLocalTimeAndFormat(x.CreatedOn, "MM/dd/yyyy HH:mm:ss")//x.CreatedOn.ToString("M/d/yyyy hh:mm:ss") + " UTC",
                                   , start == null ? string.Empty : ClientTimeZoneHelper.ConvertToLocalTimeAndFormat((DateTime)start, "MM/d/yyyy HH:mm:ss")
                                   , end == null ? string.Empty : ClientTimeZoneHelper.ConvertToLocalTimeAndFormat((DateTime)end, "MM/d/yyyy HH:mm:ss")
                                   , start == null || end == null ? string.Empty : ((TimeSpan)(end - start)).ToString(@"hh\:mm\:ss")
                                   , y.MaterialMasterNumber
                                   , x.Disposition.DispositionName
                                   , x.CompletedOn == null ? "" : ClientTimeZoneHelper.ConvertToLocalTimeAndFormat((DateTime)x.CompletedOn, "MM/dd/yyyy HH:mm:ss")
                                   , x.RequestStatus.RequestStatusName
                                   , boxStatus == null ? string.Empty : boxStatus.RequestStatus.RequestStatusName
                                   , y.DestinationLotNumber
                                   );
                    }
                    ));
            }
            catch (Exception e)
            {
                Console.WriteLine("Break at row : {0}, {1}", dt.Rows.Count, e.ToString());
                Console.ReadKey();
            }

            var newDt = dt.AsEnumerable()
              .GroupBy(ac => new
              {
                  REQUESTOR = ac.Field<string>("REQUESTOR"),
                  MRB_Number = ac.Field<string>("MRB Number"),
                  WAREHOUSE = ac.Field<string>("WAREHOUSE"),
                  LotID = ac.Field<string>("LOT ID"),
                  BoxID = ac.Field<string>("OLD BOX ID"),
                  ORIGINAL_QTY = ac.Field<string>("ORIGINAL QTY"),                  
                  NEW_BOX_ID = ac.Field<string>("NEW BOX ID"),
                  REQUEST_DATE = ac.Field<string>("REQUEST DATE"),
                  BOX_START_DT = ac.Field<string>("BOX START DT"),
                  BOX_END_DT = ac.Field<string>("BOX END DT"),
                  TIME_ELAPSED = ac.Field<string>("TIME ELAPSED (hh:mm:ss)"),
                  MM_no = ac.Field<string>("MATERIAL MASTER NO."),                                   
                  DISPOSITION = ac.Field<string>("DISPOSITION"), 
                  REQUEST_COMPLETE_DATE = ac.Field<string>("REQUEST COMPLETE DATE"),
                  REQUEST_STATUS = ac.Field<string>("REQUEST STATUS"),
                  BOX_STATUS = ac.Field<string>("BOX STATUS"),
                  FPO_LOT_ID = ac.Field<string>("FPO LOT ID")                  

              })
              .Select(g =>
              {
                  var row = dt.NewRow();

                  row[0] = g.Key.REQUESTOR;
                  row[1] = g.Key.MRB_Number;
                  row[2] = g.Key.WAREHOUSE;
                  row[3] = g.Key.LotID;
                  row[4] = g.Key.BoxID;
                  row[5] = g.Key.ORIGINAL_QTY;
                  row[6] = g.Sum(r => r.Field<Int32>("FLAGGED UNITS QTY"));
                  row[7] = g.Key.NEW_BOX_ID;
                  row[8] = g.Key.REQUEST_DATE;
                  row[9] = g.Key.BOX_START_DT;
                  row[10] = g.Key.BOX_END_DT;
                  row[11] = g.Key.TIME_ELAPSED;
                  row[12] = g.Key.MM_no;
                  row[13] = g.Key.DISPOSITION;
                  row[14] = g.Key.REQUEST_COMPLETE_DATE;
                  row[15] = g.Key.REQUEST_STATUS;
                  row[16] = g.Key.BOX_STATUS;
                  row[17] = g.Key.FPO_LOT_ID;

                  return row;
              }).CopyToDataTable();

            for (int i = 0; i < newDt.Rows.Count; i++)
            {
                builder.Append(
                   String.Join(",", new string[] {
                newDt.Rows[i]["REQUESTOR"].ToString(),
                newDt.Rows[i]["MRB Number"].ToString(),
                newDt.Rows[i]["WAREHOUSE"].ToString(),
                newDt.Rows[i]["LOT ID"].ToString(),
                newDt.Rows[i]["OLD BOX ID"].ToString(),
                newDt.Rows[i]["ORIGINAL QTY"].ToString(),
                newDt.Rows[i]["FLAGGED UNITS QTY"].ToString(),
                newDt.Rows[i]["NEW BOX ID"].ToString(),
                newDt.Rows[i]["REQUEST DATE"].ToString(),
                newDt.Rows[i]["BOX START DT"].ToString(),
                newDt.Rows[i]["BOX END DT"].ToString(),
                newDt.Rows[i]["TIME ELAPSED (hh:mm:ss)"].ToString(),
                newDt.Rows[i]["MATERIAL MASTER NO."].ToString(),
                newDt.Rows[i]["DISPOSITION"].ToString(),
                newDt.Rows[i]["REQUEST COMPLETE DATE"].ToString(),             
                newDt.Rows[i]["REQUEST STATUS"].ToString(),
                newDt.Rows[i]["BOX STATUS"].ToString(),
                newDt.Rows[i]["FPO LOT ID"].ToString(), 
                "\n" }));
            }

            var str = builder.ToString();
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }

        public ActionResult UpdateRequest(ReportIndexParentViewModel model)
        {
            var _statusList = new List<SelectListItem>();
            var _displaydate = DateTime.UtcNow.AddDays(-90);
            uow.RequestStatuses.GetAll().ToList().ForEach(x => _statusList.Add(new SelectListItem() { Value = x.RequestStatusID.ToString(), Text = x.RequestStatusName }));
            var _completedID = uow.RequestStatuses.GetAll().ToList();

            List<RequestLineItemViewModel> _requests = new List<RequestLineItemViewModel>();

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

            return View("Index", new RequestLineItemViewModel { Requests = _requests }); 
        }

    }
}