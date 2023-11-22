using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WUFT.NET.ViewModels.Admin
{
    public class FlaggedBoxLineItem
    {
        public FlaggedBoxLineItem()
        {
            VisualIDs = new List<KeyValuePair<string, string>>();
            Statuses = new List<SelectListItem>();
        }
        public string BadVisualIDCount                      { get; set; }
        public string GoodVisualIDCount                     { get; set; }
        public string GoodBoxID                             { get; set; }
        public string LotID                                 { get; set; }
        public string UnmergeLotID                          { get; set; }
        public string BoxRequestStatus                      { get; set; }
        public IEnumerable<SelectListItem> Statuses         { get; set; }
        public List<KeyValuePair<string, string>> VisualIDs { get; set; }
        public string BadBoxID                              { get; set; }
    }
}