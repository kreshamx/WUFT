using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUFT.MODEL
{
    public class QRELoad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QRELoadID { get; set; }
        public string QRELoadJobID { get; set; }
        public DateTime QRELoadDateTime { get; set; }
        public string QRELoadUserName { get; set; }
        public string LotID { get; set; }
        public string BoxID { get; set; }
        public string VisualID { get; set; }
        public string Warehouse { get; set; }
        public int DispositionID { get; set; }
        public string FPOLotID { get; set; }

        public QRELoad()
        {
            QRELoadDateTime = DateTime.UtcNow;
        } 
    }
}
