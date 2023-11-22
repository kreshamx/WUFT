using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WUFT.NET.Views.Shared
{
    [Serializable]
    public class LotBoxCount
    {
        public string LotNumber { get; set; }
        public string BoxNumber { get; set; }
        public int UnitCount { get; set; }
        public string UnmergeLotNumber { get; set; }
    }
}