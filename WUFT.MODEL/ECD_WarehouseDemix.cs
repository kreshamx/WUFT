using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace WUFT.MODEL
{
    public class ECD_WarehouseDemix
    {
        [Key]
        public int WarehouseDemixID { get; set; }
        [Required, MaxLength(20), Column(TypeName = "VARCHAR")]
        public string OriginalLotNumber { get; set; }
        [Required, MaxLength(20), Column(TypeName = "VARCHAR")]
        public string OriginalBoxNumber { get; set; }
        public int MRBID { get; set; }
        public int FacilityId { get; set; }
        [Required, MaxLength(64), Column(TypeName = "VARCHAR")]
        public string SubstrateVisualID { get; set; }
        [MaxLength(20), Column(TypeName = "VARCHAR")]
        public string DestinationLotNumber { get; set; }
        [MaxLength(20), Column(TypeName = "VARCHAR")]
        public string DestinationBoxNumber { get; set; }
        [MaxLength(20), Column(TypeName = "VARCHAR")]
        public string SCID { get; set; }
        [MaxLength(20), Column(TypeName = "VARCHAR")]
        public string MaterialMasterNumber { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int LabelQuantity { get; set; }
        public int SequenceNumber { get; set; }
        [MaxLength(20), Column(TypeName = "VARCHAR")]
        public string LoadStatus { get; set; }
        public int CarrierX { get; set; }
        public int CarrierY { get; set; }
        public DateTime ScanDateTime { get; set; }
        [MaxLength(20), Column(TypeName = "VARCHAR")]
        public string ScanType { get; set; }
        [MaxLength(20), Column(TypeName = "VARCHAR")]
        public string CarrierID { get; set; }
        public DateTime CreatedOn { get; set; }
        [MaxLength(10), Column(TypeName = "VARCHAR")]
        public string CreatedBy { get; set; }
        public DateTime LastUpdateOn { get; set; }
        [MaxLength(10), Column(TypeName = "VARCHAR")]
        public string LastUpdateBy { get; set; }
        public virtual FlagRequest FlagRequest { get; set; }
        [Index("IX_ECD_WarehouseDemix_FlagRequestID")]
        public int FlagRequestID { get; set; }
        public int DispositionID { get; set; }

        //public virtual Disposition Disposition      { get; set; }
        public string UploadedWarehouseName { get; set; }
        public bool UnitFound { get; set; }
        public bool UnitFoundButRequestStopped { get; set; }
        [MaxLength(10), Column(TypeName = "VARCHAR")]
        public string OriginalQty { get; set; }
        public int FlaggedUnitsQty { get; set; }

    }
}
