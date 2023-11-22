using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WUFT.NET.ViewModels.Shared
{
    public class RequestLineItemViewModel
    {
        public RequestLineItemViewModel()
        {
            Selected = false;
            BoxIDs = new List<Tuple<string,bool>>();
            LotIDs = new List<string>();
            Statuses = new List<SelectListItem>();
            OriginalRequestStatusID = 0;
            Requests = new List<RequestLineItemViewModel>();
            FlaggedUnits = new List<Tuple<string, List<RequestLineItemViewModel>>>();
        }
        public DateTime CreatedOn                     { get; set; }
        public DateTime LastModified                  { get; set; }
        public DateTime TimePeriodStart               { get; set; }
        public DateTime TimePeriodEnd                 { get; set; }
        public string Requestor                       { get; set; }
        public string OriginalRequestStatus           { get; set; }
        public int OriginalRequestStatusID            { get; set; }
        public List<string> TempBoxIDs                { get; set; }
        [UIHint("StringListInTableShowDuplicates")]
        public List<Tuple<string, bool>> BoxIDs       { get; set; }
        [UIHint("StringListInTable")]
        public List<string> LotIDs                    { get; set; }
        [UIHint("StringListInTable")]
        public List<string> UnmergeLotIDs             { get; set; }
        public string Disposition                     { get; set; }
        public int RequestID                          { get; set; }
        public string MRBID                           { get; set; }
        public string Warehouse                       { get; set; }
        public bool Selected                          { get; set; }
        [UIHint("CommaSeparateIntString")]
        public int UnitQty                            { get; set; }
        public int BoxQty                             { get; set; } //vsharmx : TASK7720599 
        public IEnumerable<SelectListItem> Statuses   { get; set; }
        public string SelectedStatus                  { get; set; }
        public List<RequestLineItemViewModel> Requests { get; set; }
        public List<Tuple<string, List<RequestLineItemViewModel>>> FlaggedUnits { get; set; }
        public string OriginalMRB { get; set; }
    }
}