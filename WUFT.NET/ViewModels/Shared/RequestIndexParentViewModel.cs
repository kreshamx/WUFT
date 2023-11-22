using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WUFT.NET.ViewModels.Shared
{
    public class RequestIndexParentViewModel
    {
        public RequestIndexParentViewModel()
        {
            Requests = new List<RequestLineItemViewModel>();
            Warehouses = new List<SelectListItem>();

            //Changes for FS-INC100004856
            TimePeriodEnd = DateTime.Now.Date;
            TimePeriodStart = DateTime.UtcNow.AddDays(-90);
        }

        public List<RequestLineItemViewModel> Requests { get; set; }
        public IEnumerable<SelectListItem> Warehouses { get; set; }
        public string SelectedWarehouse { get; set; }
        public string IdList { get; set; }

        public DateTime TimePeriodStart { get; set; }
        public DateTime TimePeriodEnd { get; set; }

        public IEnumerable<int> getSelectedIds()
        {
            return (from p in this.Requests where p.Selected select p.RequestID).ToList();
        }
    }
}