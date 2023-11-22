using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.Service
{
    public class EndDemixWlotRequestModel
    {
        public EndDemixWlotRequestModel()
        {
            FoundUnits = new List<string>();
        }
        public string LotNumber { get; set; }
        public string UnmergeLotNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string Facility { get; set; }
        public string BoxNumber { get; set; }
        public string Mode { get; set; }
        public List<string> FoundUnits { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}