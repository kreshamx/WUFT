using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.QRE
{
    public class UploadUnits
    {
        [UIHint("UploadUnitsTable")]
        public List<UploadUnitsLineItem> Units { get; set; }
        [DisplayFormat(DataFormatString = "#,##0")]
        public string BoxCount                 { get; set; }
        [DisplayFormat(DataFormatString = "#,##0")]
        public string LotCount                 { get; set; }
        [DisplayFormat(DataFormatString="N0")]
        public string UnitQty                  { get; set; }
        public string WarehouseName            { get; set; }
        public string Disposition              { get; set; }
        public string CollapseIdentifier       { get; set; }
        [DisplayFormat(DataFormatString = "#,##0")]
        public string UnmergeLotCount          { get; set; }
    }
}