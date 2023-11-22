using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUFT.DATA.Fakes;
using WUFT.MODEL;

namespace WUFT.TEST
{
    public class ControllerBaseTest
    {
        protected StubIUnitOfWork stub;
        private List<FlagRequest> testRequests;
        private List<ECD_WarehouseDemix> testUnits;
        private List<Disposition> testStatuses;

        public ControllerBaseTest()
        {
            CreateStatuses();
            CreateECD_WarehouseDemixUnits();
            CreateRequests();
            stub = StubUnitOfWork();
        }

        private void CreateStatuses()
        {
            testStatuses = new List<Disposition>();
            testStatuses.Add(new Disposition { DispositionID = 7, DispositionName = "Unmerge" });
            testStatuses.Add(new Disposition {DispositionID = 8, DispositionName = "Scrap"});
            testStatuses.Add(new Disposition {DispositionID = 9, DispositionName = "Release"});
        }

        public void CreateRequests()
        {
            testRequests = new List<FlagRequest>();
            var _oldDate = DateTime.UtcNow.AddDays(-30);
            testRequests.Add(new FlagRequest { FlagRequestID = 1, WarehouseName = "AZ04", LastModified = DateTime.UtcNow, LastModifiedBy = "ALFISCHE", FlaggedUnits = testUnits.Where(x => x.FlagRequestID == 1).ToList(), DispositionID = 1, Disposition = testStatuses.FirstOrDefault(x => x.DispositionID == 1), RequestStatusID = 0, CreatedOn = DateTime.UtcNow, CreatedBy = "ALFISCHE", MRBID = "1111111" });
            testRequests.Add(new FlagRequest { FlagRequestID = 2, WarehouseName = "AZ04", LastModified = DateTime.UtcNow, LastModifiedBy = "ALFISCHE", FlaggedUnits = testUnits.Where(x => x.FlagRequestID == 2).ToList(), DispositionID = 1, Disposition = testStatuses.FirstOrDefault(x => x.DispositionID == 1), RequestStatusID = 0, CreatedOn = DateTime.UtcNow, CreatedBy = "ALFISCHE", MRBID = "2222222" });
            testRequests.Add(new FlagRequest { FlagRequestID = 3, WarehouseName = "CNA2", LastModified = DateTime.UtcNow, LastModifiedBy = "ALFISCHE", FlaggedUnits = testUnits.Where(x => x.FlagRequestID == 3).ToList(), DispositionID = 3, Disposition = testStatuses.FirstOrDefault(x => x.DispositionID == 3), RequestStatusID = 0, CreatedOn = DateTime.UtcNow, CreatedBy = "ALFISCHE", MRBID = "6666666" });
            testRequests.Add(new FlagRequest { FlagRequestID = 4, WarehouseName = "CNA2", LastModified = DateTime.UtcNow, LastModifiedBy = "ALFISCHE", FlaggedUnits = testUnits.Where(x => x.FlagRequestID == 4).ToList(), DispositionID = 4, Disposition = testStatuses.FirstOrDefault(x => x.DispositionID == 4), RequestStatusID = 2, CreatedOn = DateTime.UtcNow, CreatedBy = "ALFISCHE", MRBID = "6666666" });
            testRequests.Add(new FlagRequest { FlagRequestID = 5, WarehouseName = "CNA2", LastModified = _oldDate, LastModifiedBy = "ALFISCHE", FlaggedUnits = testUnits.Where(x => x.FlagRequestID == 5).ToList(), DispositionID = 1, Disposition = testStatuses.FirstOrDefault(x => x.DispositionID == 1), RequestStatusID = 2, CreatedOn = DateTime.UtcNow, CreatedBy = "ALFISCHE", MRBID = "6666666" });
        }                                                         

        public void CreateECD_WarehouseDemixUnits()
        {
            testUnits = new List<ECD_WarehouseDemix>();
            testUnits.Add(new ECD_WarehouseDemix { WarehouseDemixID = 1, MRBID = 0, FacilityId = 0, SubstrateVisualID = "3E309209A0972", DispositionID = 7, DestinationLotNumber = null, DestinationBoxNumber = null, SCID = null, MaterialMasterNumber = null, StartDateTime = DateTime.UtcNow, EndDateTime = DateTime.UtcNow, LabelQuantity = 0, SequenceNumber = 0, LoadStatus = null, CarrierX = 0, CarrierY = 0, ScanDateTime = DateTime.Now, ScanType = null, CarrierID = null, CreatedOn = DateTime.Now, CreatedBy = null, LastUpdateOn = DateTime.Now, OriginalLotNumber = "N304346A", OriginalBoxNumber = "RVR16703", FlagRequestID = 1 });
            testUnits.Add(new ECD_WarehouseDemix { WarehouseDemixID = 2, MRBID = 0, FacilityId = 0, SubstrateVisualID = "3E309209A2467", DispositionID = 7, DestinationLotNumber = null, DestinationBoxNumber = null, SCID = null, MaterialMasterNumber = null, StartDateTime = DateTime.UtcNow, EndDateTime = DateTime.UtcNow, LabelQuantity = 0, SequenceNumber = 0, LoadStatus = null, CarrierX = 0, CarrierY = 0, ScanDateTime = DateTime.Now, ScanType = null, CarrierID = null, CreatedOn = DateTime.Now, CreatedBy = null, LastUpdateOn = DateTime.Now, OriginalLotNumber = "N304346A", OriginalBoxNumber = "RVR16703", FlagRequestID = 1 });
            testUnits.Add(new ECD_WarehouseDemix { WarehouseDemixID = 3, MRBID = 0, FacilityId = 0, SubstrateVisualID = "3E309209A2480", DispositionID = 7, DestinationLotNumber = null, DestinationBoxNumber = null, SCID = null, MaterialMasterNumber = null, StartDateTime = DateTime.UtcNow, EndDateTime = DateTime.UtcNow, LabelQuantity = 0, SequenceNumber = 0, LoadStatus = null, CarrierX = 0, CarrierY = 0, ScanDateTime = DateTime.Now, ScanType = null, CarrierID = null, CreatedOn = DateTime.Now, CreatedBy = null, LastUpdateOn = DateTime.Now, OriginalLotNumber = "N304346A", OriginalBoxNumber = "RVR16704", FlagRequestID = 2 });
            testUnits.Add(new ECD_WarehouseDemix { WarehouseDemixID = 4, MRBID = 0, FacilityId = 0, SubstrateVisualID = "3E309209A2438", DispositionID = 7, DestinationLotNumber = null, DestinationBoxNumber = null, SCID = null, MaterialMasterNumber = null, StartDateTime = DateTime.UtcNow, EndDateTime = DateTime.UtcNow, LabelQuantity = 0, SequenceNumber = 0, LoadStatus = null, CarrierX = 0, CarrierY = 0, ScanDateTime = DateTime.Now, ScanType = null, CarrierID = null, CreatedOn = DateTime.Now, CreatedBy = null, LastUpdateOn = DateTime.Now, OriginalLotNumber = "N304346A", OriginalBoxNumber = "RVR16704", FlagRequestID = 2 });
            testUnits.Add(new ECD_WarehouseDemix { WarehouseDemixID = 5, MRBID = 0, FacilityId = 0, SubstrateVisualID = "3E309209A2444", DispositionID = 9, DestinationLotNumber = null, DestinationBoxNumber = null, SCID = null, MaterialMasterNumber = null, StartDateTime = DateTime.UtcNow, EndDateTime = DateTime.UtcNow, LabelQuantity = 0, SequenceNumber = 0, LoadStatus = null, CarrierX = 0, CarrierY = 0, ScanDateTime = DateTime.Now, ScanType = null, CarrierID = null, CreatedOn = DateTime.Now, CreatedBy = null, LastUpdateOn = DateTime.Now, OriginalLotNumber = "N3043760", OriginalBoxNumber = "RVR16555", FlagRequestID = 4 });
            testUnits.Add(new ECD_WarehouseDemix { WarehouseDemixID = 6, MRBID = 0, FacilityId = 0, SubstrateVisualID = "3E309209A2444", DispositionID = 7, DestinationLotNumber = null, DestinationBoxNumber = null, SCID = null, MaterialMasterNumber = null, StartDateTime = DateTime.UtcNow, EndDateTime = DateTime.UtcNow, LabelQuantity = 0, SequenceNumber = 0, LoadStatus = null, CarrierX = 0, CarrierY = 0, ScanDateTime = DateTime.Now, ScanType = null, CarrierID = null, CreatedOn = DateTime.Now, CreatedBy = null, LastUpdateOn = DateTime.Now, OriginalLotNumber = "N3044070", OriginalBoxNumber = "RVR16666", FlagRequestID = 5 });
        }

        private StubIUnitOfWork StubUnitOfWork()
        {
            return new StubIUnitOfWork
            {
                WarehousesGet = () => StubWarehouseRepository(),
                FlagRequestsGet = () => StubRequestRepository(),
                DispositionsGet = () => StubStatusRepository(),
                WarehouseDemixesGetECD = () => StubECDDemixRepository()
            };
        }

        private StubIRepository<ECD_WarehouseDemix> StubECDDemixRepository()
        {
            return new StubIRepository<ECD_WarehouseDemix>
            {
                GetAll = () =>
                    {
                        return testUnits.AsQueryable();
                    }
            };
        }

        private StubIRepository<Warehouse> StubWarehouseRepository()
        {
            return new StubIRepository<Warehouse>
            {
                GetAll = () =>
                    {
                        return new List<Warehouse>()
                        {
                            new Warehouse{PlantCode = "AZ04",	PrimaryEmailAddresses = "customer.service.arizona.az04.az05@intel.com",	CCEmailAddresses = null,	SiteCode = "TEST"},
                            new Warehouse{PlantCode = "CNA2",	PrimaryEmailAddresses = "inventory.control.cna2@intel.com",	CCEmailAddresses = "inventory.control.cn20@intel.com",	SiteCode = "TEST"}
                        }.AsQueryable();
                    }
            };
        }

        private StubIRepository<Disposition> StubStatusRepository()
        {
            return new StubIRepository<Disposition>
            {
                GetAll = () =>
                    {
                        return testStatuses.AsQueryable();
                    }
            };
        }

        private StubIRepository<FlagRequest> StubRequestRepository()
        {
            return new StubIRepository<FlagRequest>
            {
                GetAll = () =>
                    {
                        return testRequests.AsQueryable();
                    }
            };
        }

        public StubIRepository<T> StubRepository<T>() where T : class
        {
            return new StubIRepository<T>
            {
                GetAll = () =>
                {
                    return new List<T>().AsQueryable();
                }
            };
        }
    }
}
