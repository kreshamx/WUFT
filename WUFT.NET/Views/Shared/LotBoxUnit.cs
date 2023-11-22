using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WUFT.NET.Views.Shared
{
    [Serializable]
    public class LotBoxUnit
    {
        public string LotNumber { get; set; }
        public string BoxNumber { get; set; }
        public string VisualID { get; set; }
    }
}