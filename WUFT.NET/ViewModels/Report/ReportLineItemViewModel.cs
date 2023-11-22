using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.Report
{
    public class ReportLineItemViewModel
    {
        public ReportLineItemViewModel()
        {
            Selected = false;
            BoxIDs = new List<string>();
            LotIDs = new List<string>();
            //Requests = new List<ReportLineItemViewModel>();
        }
        public DateTime CreatedOn          { get; set; }
        public DateTime LastModified       { get; set; }
        public string Requestor          { get; set; }
        public string RequestStatus { get; set; }
        [UIHint("StringListInTable")]
        public List<string> BoxIDs       { get; set; }
        [UIHint("StringListInTable")]
        public List<string> LotIDs       { get; set; }
        [UIHint("StringListInTable")]
        public List<string> UnmergeLotIDs { get; set; }
        public string Disposition             { get; set; }
        public int RequestID             { get; set; }
        public string MRBID            { get; set; }
        public string Warehouse   { get; set; }
        public bool Selected             { get; set; }
        public int UnitQty            { get; set; }
        //public List<ReportLineItemViewModel> Requests { get; set; }
        public string OriginalMRB { get; set; }
    }
}