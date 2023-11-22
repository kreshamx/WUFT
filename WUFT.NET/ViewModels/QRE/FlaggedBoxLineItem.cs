using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.QRE
{
    public class FlaggedBoxLineItem
    {
        public FlaggedBoxLineItem()
        {
            VisualIDs = new List<KeyValuePair<string, string>>();
        }
        public string BadVisualIDCount                      { get; set; }
        public string GoodVisualIDCount                     { get; set; }
        public string GoodBoxID                             { get; set; }
        public string LotID                                 { get; set; }
        public string UnmergeLotID                          { get; set; }
        public string BoxRequestStatus                      { get; set; }
        public List<KeyValuePair<string, string>> VisualIDs { get; set; }
        public string BadBoxID                              { get; set; }
    }
}