using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUFT.MODEL
{
    public class FlagRequest
    {
        public int FlagRequestID                                        { get; set; }
        public DateTime LastModified                                    { get; set; }
        [Required, ForeignKey("User")]
        public string LastModifiedBy                                    { get; set; }
        public virtual WUFTUser User                                    { get; set; }
        public virtual List<ECD_WarehouseDemix> FlaggedUnits            { get; set; }
        [Required,ForeignKey("Disposition")]
        public int DispositionID                                        { get; set; }
        public virtual Disposition Disposition                          { get; set; }
        [Required,ForeignKey("RequestStatus")]
        public int RequestStatusID                                      { get; set; }
        public virtual RequestStatus RequestStatus                      { get; set; }
        public DateTime CreatedOn                                       { get; set; }
        [Required]
        public string CreatedBy                                         { get; set; }
        [Required]
        public string MRBID                                             { get; set; }       
        public string OriginalMRB                                       { get; set; }
        public virtual Warehouse Warehouse                              { get; set; }
        [Required,ForeignKey("Warehouse")]
        public string WarehouseName                                     { get; set; }
        public DateTime? CompletedOn                                    { get; set; }
        public string CompletedBy                                       { get; set; }
        public DateTime? StartTime                                      { get; set; }
        public DateTime? EndTime                                        { get; set; }
    }
}
