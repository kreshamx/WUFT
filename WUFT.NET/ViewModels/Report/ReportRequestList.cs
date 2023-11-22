using System.Collections.Generic;
using System.Linq;

namespace WUFT.NET.ViewModels.Report
{
    public class ReportRequestList
    {
        public ReportRequestList()
        {
            Requests = new List<ReportLineItemViewModel>();

        }
        public List<ReportLineItemViewModel> Requests { get; set; }

        public IEnumerable<int> getSelectedIds()
        {
            return (from p in this.Requests where p.Selected select p.RequestID).ToList();
        }
    }
}