using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WUFT.NET.ViewModels.Service
{
    [XmlRoot("UpdateResponse")]
    public class UpdateResponseViewModel
    {
        [XmlElement("Status")]
        public string Status { get; set; }
    }
}