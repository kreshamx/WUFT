using WUFT.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUFT.DATA
{
    public interface IUnitOfWork
    {
        IRepository<Warehouse> Warehouses                    { get; }
        IRepository<ECD_WarehouseDemix> ECD_WarehouseDemixes { get; }
        IRepository<FlagRequest> FlagRequests                { get; }
        IRepository<Disposition> Dispositions                { get; }
        IRepository<QRELoad> QRELoads                        { get; }
        IRepository<RequestStatus> RequestStatuses           { get; }
        IRepository<StationController> StationControllers    { get; }
        IRepository<WUFTUser> Users                          { get; }
        IRepository<BoxRequestStatus> BoxRequestStatuses     { get; }
        IRepository<ErrorEmailCC> ErrorEmailCCs              { get; }


        void SaveChanges();
        void SaveChangesAsync();
    }
}
