using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WUFT.NET.ViewModels.Service
{
    [XmlRoot("UpdateUnitContainmentStatus")]
    public class Update_originalQuantity_flaggedUnit
    {
        public Update_originalQuantity_flaggedUnit()
        {
            Lots = new List<Lot>();
        }

        [XmlElement("Lot")]
        public List<Lot> Lots { get; set; }
        public string FlaggedUnits { get; set; }

        public class Lot
        {
            [XmlAttribute]
            public string Quantity { get; set; }
            [XmlAttribute]
            public string LotNumber { get; set; }
            [XmlAttribute]
            public string ReferenceNumber { get; set; }

            [XmlElement("Box")]
            public List<Box> Boxes { get; set; }

            public Lot()
            {
                Boxes = new List<Box>();
            }
        }

        public class Box
        {
            [XmlAttribute]
            public string BoxNumber { get; set; }
        }
    }

}