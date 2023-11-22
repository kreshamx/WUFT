using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.QRE
{
    public class UploadUnitsLineItem
    {
        public string BoxID { get; set; }
        public string LotID { get; set; }
        [DisplayFormat(DataFormatString = "N0")]
        public string UnitQty { get; set; }
        public string UnmergeLotID { get; set; }

    }
}