using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.Operator
{
    public class FlaggedUnitLineItem
    {
        public string VisualIDCount  { get; set; }
        public string BoxID     { get; set; }
        public string LotID     { get; set; }
        public string UnmergeLotID { get; set; }
        public string BoxRequestStatus { get; set; }
        public int FlagRequestID { get; set; }
    }
}