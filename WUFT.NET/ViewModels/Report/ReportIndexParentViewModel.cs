using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WUFT.NET.ViewModels.Report
{
    public class ReportIndexParentViewModel
    {
        public ReportIndexParentViewModel()
        {
            Requests = new ReportRequestList();
            Warehouses = new List<SelectListItem>();

            //Changes for FS-INC100004856
            TimePeriodEnd = DateTime.Now.Date;
            TimePeriodStart = DateTime.UtcNow.AddDays(-90);

        }
        public string IDs { get; set; }
        public DateTime TimePeriodStart { get; set; }
        public DateTime TimePeriodEnd { get; set; }
        [UIHint("ReportTable")]
        public ReportRequestList Requests { get; set; }
        public IEnumerable<SelectListItem> Warehouses { get; set; }
        public string SelectedWarehouse { get; set; }
        
    }
}