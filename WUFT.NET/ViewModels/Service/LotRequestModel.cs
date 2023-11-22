using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WUFT.NET.ViewModels.Service
{
    public class LotRequestModel
    {
        public string LotNumber { get; set; }
        public string UnmergeLotNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string Facility { get; set; }
        public string BoxNumber { get; set; }
        public string Quantity { get; set; }
    }
}