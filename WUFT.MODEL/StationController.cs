using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WUFT.MODEL
{
    public class StationController
    {
        [Key]
        public int StationControllerID { get; set; }
        public string StationControllerName { get; set; }
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string UserDomain { get; set; }
        public string StationControllerSite { get; set; }
        public string AppVersion { get; set; }
        public string WindowsVersion { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
