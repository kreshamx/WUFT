using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUFT.MODEL
{
    public class Disposition
    {
        [Index("IX_DispositionID")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DispositionID { get; set; }
        public string DispositionName { get; set; }
    }
}
