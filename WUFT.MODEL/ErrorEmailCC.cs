using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WUFT.MODEL
{
    public class ErrorEmailCC
    {
        [Key]
        public int ErrorEmailCCID { get; set; }
        public string EmailAddress { get; set; }

    }
}
