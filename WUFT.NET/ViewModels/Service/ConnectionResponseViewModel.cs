using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WUFT.NET.ViewModels.Service
{
    [XmlRoot("ConnectionResponse")]
    public class ConnectionResponseViewModel
    {
        [XmlElement("Status")]
        public string Status { get; set; }
        [XmlElement("StationControllerName")]
        public string StationControllerName { get; set; }
    }
}