using System.Data.Entity;
using WUFT.MODEL;

namespace WUFT.EF
{
    public class WUFTDbContext : DbContext
    {
        public WUFTDbContext()
            : base("name=WUFTDbContext")
        {
        }

        public DbSet<Warehouse> Warehouses                    { get; set; }
        public DbSet<ECD_WarehouseDemix> ECD_WarehouseDemixes { get; set; }
        public DbSet<FlagRequest> FlagRequests                { get; set; }
        public DbSet<Disposition> Dispositions                { get; set; }
        public DbSet<QRELoad> QRELoads                        { get; set; }
        public DbSet<RequestStatus> RequestStatuses           { get; set; }
        public DbSet<StationController> StationControllers    { get; set; }
        public DbSet<WUFTUser> Users                          { get; set; }
        public DbSet<BoxRequestStatus> BoxRequestStatuses     { get; set; }
        public DbSet<ErrorEmailCC> ErrorEmailCCs              { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FlagRequest>().HasRequired(a => a.RequestStatus).WithMany().HasForeignKey(c => c.RequestStatusID).WillCascadeOnDelete(false);
        }

    }


}
