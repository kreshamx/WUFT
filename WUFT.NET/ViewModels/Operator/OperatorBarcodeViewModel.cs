using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.Operator
{
    public class OperatorBarcodeViewModel
    {
        public OperatorBarcodeViewModel()
        {
            InvalidID = false;
            MRBID = "";
            Disposition = "";
            CreatedBy = "";
            CreatedOn = DateTime.MinValue;
            Warehouse = "";
            Mode = "";
            FlaggedUnitGroups = new List<FlaggedUnitGroup>();
        }
        public List<FlaggedUnitGroup> FlaggedUnitGroups { get; set; }
        public bool InvalidID { get; set; }
        [Display(Name = "MRB Number")]
        public string MRBID { get; set; }
        public string Disposition { get; set; }
        [Display(Name = "Request Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Request Created On")]
        public DateTime CreatedOn { get; set; }
        public string Warehouse { get; set; }
        public int RequestID { get; set; }
        public string Mode { get; set; }
        

    }
}