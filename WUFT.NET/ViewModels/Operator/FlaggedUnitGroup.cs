using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.Operator
{
    public class FlaggedUnitGroup
    {
        [Display(Name = "MRB Number")]
        public string MRBID            { get; set; }
        [Display(Name = "Box ID")]
        public string BoxID            { get; set; }
        [Display(Name = "Lot ID")]
        public string LotID            { get; set; }
        [Display(Name = "Unmerge Lot ID")]
        public string UnmergeLotID     { get; set; }
        [Display(Name = "Flag Qty")]
        public int Quantity            { get; set; }
        public string Disposition      { get; set; }
        [Display(Name = "Request Created By")]
        public string CreatedBy        { get; set; }
        [Display(Name = "Request Created On")]
        public DateTime CreatedOn      { get; set; }
        public string Warehouse        { get; set; }
        public string BoxRequestStatus { get; set; }
    }
}