using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WUFT.NET.ViewModels.Service
{
    [XmlRoot("UnitContainmentDataResponse")]
    public class LotResponseModel
    {
        public LotResponseModel()
        {
            Lots = new List<Lot>();
        }

        [XmlElement("Lot")]
        public List<Lot> Lots { get; set; }

        public class Lot
        {
            [XmlAttribute]
            public string LotNumber { get; set; }
            [XmlAttribute]
            public string UnmergeLotNumber { get; set; }
            [XmlAttribute]
            public string ReferenceNumber { get; set; }
            [XmlAttribute]
            public string Facility { get; set; }
            [XmlAttribute]
            public string Quantity { get; set; }
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
            [XmlElement("Unit")]
            public List<Unit> Units { get; set; }

            public Box()
            {
                Units = new List<Unit>();
            }
        }

        public class Unit
        {
            [XmlAttribute]
            public string VisualId { get; set; }
            [XmlAttribute]
            public string Status { get; set; }

            public Unit()
            {
                VisualId = "";
                Status = "";
            }
        }

    }

}