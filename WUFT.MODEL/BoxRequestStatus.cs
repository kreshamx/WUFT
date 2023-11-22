using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUFT.MODEL
{
    public class BoxRequestStatus
    {
        [Key]
        [Required]
        public int BoxRequestStatusID              { get; set; }
        public string BoxNumber                    { get; set; }
        [ForeignKey("RequestStatus")]
        public int RequestStatusID                 { get; set; }
        public virtual RequestStatus RequestStatus { get; set; }
        [ForeignKey("FlagRequest")]
        public int FlagRequestID                   { get; set; }
        public virtual FlagRequest FlagRequest     { get; set; }
        public DateTime CreatedOn                  { get; set; }
        public DateTime LastModifiedOn             { get; set; }
        public DateTime? CompletedOn               { get; set; }
        public int? GoodBoxUnitQty                 { get; set; }
        public string LastModifiedBy               { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        //public int BoxScanQty { get; set; }
    }
}

