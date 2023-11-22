using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WUFT.NET.ViewModels.Service
{
    [XmlRoot("ConnectionTestRequest")]
    public class ConnectionTestViewModel
    {
        public string StationControllerID { get; set; }
        public string StationControllerName { get; set; }
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string UserDomain { get; set; }
        public string StationControllerSite { get; set; }
        public string AppVersion { get; set; }
        public string WindowsVersion { get; set; }
    }
}