using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WUFT.MODEL
{
    public class Warehouse
    {
        [Key]
        public string PlantCode { get; set; }
        public string SiteCode { get; set; }
        public string PrimaryEmailAddresses { get; set; }
        public string CCEmailAddresses { get; set; }
    }
}
