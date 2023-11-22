using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.Emails
{
    [Serializable]
    public class CompletedBoxSummary
    {
        public string BadVisualIDCount  { get; set; }
        public string GoodVisualIDCount { get; set; }
        public string GoodBoxID         { get; set; }
        public string LotID             { get; set; }
        public string UnmergeLotID      { get; set; }
        public string BadBoxID          { get; set; }
    }
}