using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUFT.MODEL
{
    public class WUFTUser
    {
        [Key]
        [StringLength(10)]
        public string IdSid        { get; set; }
        [Required]
        public string FullName     { get; set; }
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string WWID         { get; set; }
    }
}
