using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WUFT.NET.ViewModels.Service
{
    [XmlRoot("UpdateUnitContainmentStatus")]
    public class UpdateStatusViewModel
    {
        public UpdateStatusViewModel()
        {
            Lots = new List<Lot>();
        }

        [XmlElement("Lot")]
        public List<Lot> Lots { get; set; }
        [XmlAttribute]
        public DateTime StartTime { get; set; }
        [XmlAttribute]
        public DateTime EndTime { get; set; }
        public class Lot
        {
            [XmlAttribute]
            public string LotNumber { get; set; }
            [XmlAttribute]
            public string ReferenceNumber { get; set; }
            [XmlAttribute]
            public string Facility { get; set; }
            [XmlAttribute]
            public string SCID { get; set; }
            [XmlAttribute]
            public string MaterialMasterNumber { get; set; }


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
            [XmlAttribute]
            public string StartDateTime { get; set; }
            [XmlAttribute]
            public string EndDateTime { get; set; }
            [XmlAttribute]
            public string LabelQuantity { get; set; }
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
            public string SequenceNumber { get; set; }
            [XmlAttribute]
            public string Status { get; set; }
            [XmlAttribute]
            public string LoadStatus { get; set; }
            [XmlAttribute]
            public string CarrierX { get; set; }
            [XmlAttribute]
            public string CarrierY { get; set; }
            [XmlAttribute]
            public string ScanDateTime { get; set; }
            [XmlAttribute]
            public string CarrierId { get; set; }

            public Unit()
            {
                VisualId = "";
                SequenceNumber = "";
                LoadStatus = "";
                CarrierX = "";
                CarrierY = "";
                ScanDateTime = "";
                CarrierId = "";
                Status = "";
            }
        }

    }
}