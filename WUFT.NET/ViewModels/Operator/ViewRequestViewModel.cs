using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.Operator
{
    public class ViewRequestViewModel
    {
        public ViewRequestViewModel()
        {
            InvalidID = false;
            FlaggedUnits = new List<Tuple<string, List<FlaggedUnitLineItem>>>();
        }
        public List<Tuple<string, List<FlaggedUnitLineItem>>> FlaggedUnits   { get; set; }
        public bool InvalidID                           { get; set; }
        [Display(Name="MRB Number")]
        public string MRBID                             { get; set; }
        public string Disposition                       { get; set; }
        [Display(Name="Request Created By")]
        public string CreatedBy                         { get; set; }
        [Display(Name="Request Created On")]
        public DateTime CreatedOn                       { get; set; }
        public string Warehouse                         { get; set; }
        public int RequestID                            { get; set; }
        public string BoxCount                          { get; set; }
        [Display(Name="Request Status")]
        public string RequestStatus { get; set; }
        public string PercentDone                       { get; set; }
    }
}