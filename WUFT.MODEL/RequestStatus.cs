using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUFT.MODEL
{
    public class RequestStatus
    {
        public int RequestStatusID { get; set; }
        public string RequestStatusName { get; set; }
    }
}
