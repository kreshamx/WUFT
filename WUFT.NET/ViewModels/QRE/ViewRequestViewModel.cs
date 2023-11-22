using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.QRE
{
    public class ViewRequestViewModel
    {
        public ViewRequestViewModel()
        {
            InvalidID = false;
            FlaggedBoxes = new List<FlaggedBoxLineItem>();
        }
        public List<FlaggedBoxLineItem> FlaggedBoxes         { get; set; }
        public bool InvalidID                                { get; set; }
        [Display(Name="MRB Number")]
        public string MRBID                                  { get; set; }
        public string Disposition                            { get; set; }
        [Display(Name="Request Created By")]
        public string CreatedBy                              { get; set; }
        [Display(Name="Request Last Updated On")]
        public DateTime LastModifiedOn                       { get; set; }
        public string Warehouse                              { get; set; }
        public int RequestID                                 { get; set; }
        [Display(Name ="Box Count")]
        public string BoxCount                               { get; set; }
        [Display(Name="Request Status")]
        public string RequestStatus                          { get; set; }
        public string PercentDone                            { get; set; }
        public string CreatedByEmail { get; set; }
        public List<FlaggedBoxLineItem> CompletedBoxes { get; set; }
    }
}