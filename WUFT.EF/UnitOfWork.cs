using System;
using System.Data.Entity;
using WUFT.DATA;
using WUFT.MODEL;


namespace WUFT.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        public WUFTDbContext DbContext { get; set; }
        public UnitOfWork()
        {
            CreateDbContext();
        }

        //Add tables here
        public IRepository<Warehouse> Warehouses                    { get { return new EfRepository<Warehouse>(DbContext); } }
        public IRepository<ECD_WarehouseDemix> ECD_WarehouseDemixes { get { return new EfRepository<ECD_WarehouseDemix>(DbContext); } }
        public IRepository<FlagRequest> FlagRequests                { get { return new EfRepository<FlagRequest>(DbContext); } }
        public IRepository<Disposition> Dispositions                { get { return new EfRepository<Disposition>(DbContext); } }
        public IRepository<QRELoad> QRELoads                        { get { return new EfRepository<QRELoad>(DbContext); } }
        public IRepository<RequestStatus> RequestStatuses           { get { return new EfRepository<RequestStatus>(DbContext); } }
        public IRepository<StationController> StationControllers    { get { return new EfRepository<StationController>(DbContext); } }
        public IRepository<WUFTUser> Users                          { get { return new EfRepository<WUFTUser>(DbContext); } }
        public IRepository<BoxRequestStatus> BoxRequestStatuses     { get { return new EfRepository<BoxRequestStatus>(DbContext); } }
        public IRepository<ErrorEmailCC> ErrorEmailCCs              { get { return new EfRepository<ErrorEmailCC>(DbContext); } }
        
        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }

        public void SaveChangesAsync()
        {
            DbContext.SaveChangesAsync();
        }

        protected void CreateDbContext()
        {
            DbContext = new WUFTDbContext();

            DbContext.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            DbContext.Configuration.LazyLoadingEnabled = true;

            // Because Web API will perform validation, we don't need/want EF to do so
            DbContext.Configuration.ValidateOnSaveEnabled = false;

            //DbContext.Configuration.AutoDetectChangesEnabled = false;
            // We won't use this performance tweak because we don't need
            // the extra performance and, when autodetect is false,
            // we'd have to be careful. We're not being that careful.
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }
    }
}
