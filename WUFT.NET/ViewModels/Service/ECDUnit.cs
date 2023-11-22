using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WUFT.MODEL;

namespace WUFT.NET.ViewModels.Service
{
    public class ECDUnit
    {
        public string LotID    { get; set; }
        public string BoxID    { get; set; }
        public string UnitQty { get; set; }
        public bool Selected               { get; set; }
        public string RequestType          { get; set; }
    }
}