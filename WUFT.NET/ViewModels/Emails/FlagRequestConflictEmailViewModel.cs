using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WUFT.NET.ViewModels.Emails
{
    [Serializable]
    public class FlagRequestConflictEmailViewModel
    {
        public String Disposition             { get; set; }
        public String OriginalRequestor          { get; set; }
        public String NewRequestor               { get; set; }
        public String WarehouseName              { get; set; }
        public String OriginalMRBID              { get; set; }
        public String NewMRBID                   { get; set; }
        public List<String> ConflictVisualIDList { get; set; } 
        public Dictionary<string, string> ConflictBoxID { get; set; }  //vsharm6x
        public String URL                        { get; set; }
    }
}