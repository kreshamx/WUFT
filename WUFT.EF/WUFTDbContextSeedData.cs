using System.Data.Entity;
using System.Data.Entity.Migrations;
using WUFT.MODEL;

namespace WUFT.EF
{
    public partial class WUFTDbContextSeedData : CreateDatabaseIfNotExists<WUFTDbContext>
    {
        protected override void Seed(WUFTDbContext context)
        {
            SeedAll(context);
        }

        public void SeedAll(WUFTDbContext context)
        {
            this.SeedWarehouses(context);
           // this.CreateStoredProcedures(context);
            this.SeedRequestStatuses(context);
            this.SeedWUFTUsers(context);
            this.SeedDispositions(context);
            this.SeedErrorEmailCCs(context);
        }

        private void SeedDispositions(WUFT.EF.WUFTDbContext context)
        {
            context.Dispositions.AddOrUpdate(new Disposition { DispositionID = 7, DispositionName = "Unmerge" });
            context.Dispositions.AddOrUpdate(new Disposition { DispositionID = 8, DispositionName = "Demix for Scrap" });
            context.Dispositions.AddOrUpdate(new Disposition { DispositionID = 9, DispositionName = "Release" });
        }

        private void SeedRequestStatuses(WUFT.EF.WUFTDbContext context)
        {
            context.RequestStatuses.AddOrUpdate(new RequestStatus { RequestStatusID = 0, RequestStatusName = "New Request" });
            context.RequestStatuses.AddOrUpdate(new RequestStatus { RequestStatusID = 1, RequestStatusName = "In Process" });
            context.RequestStatuses.AddOrUpdate(new RequestStatus { RequestStatusID = 2, RequestStatusName = "Completed" });
            context.RequestStatuses.AddOrUpdate(new RequestStatus { RequestStatusID = 3, RequestStatusName = "Completed with Units Missing" });
            context.RequestStatuses.AddOrUpdate(new RequestStatus { RequestStatusID = 4, RequestStatusName = "Stopped with Units Missing" });
        }

        private void SeedWUFTUsers(WUFT.EF.WUFTDbContext context)
        {
            context.Users.AddOrUpdate(new WUFTUser { IdSid = "bzhan10", WWID = "10690331", EmailAddress = "benedict.zhang@intel.com", FullName = "ZHANG, BO (BENE)" });
            context.Users.AddOrUpdate(new WUFTUser { IdSid = "rshanm3x", WWID = "11661057", EmailAddress = "rajkumarx.shanmugam@intel.com", FullName = "RAJKUMAR, SHANMUGAM" });
            context.Users.AddOrUpdate(new WUFTUser { IdSid = "eestewar", WWID = "11395507", EmailAddress = "eric.e.stewart@intel.com", FullName = "STEWART, ERIC E" });
            context.Users.AddOrUpdate(new WUFTUser { IdSid = "sys_dcult", WWID = "00000000", EmailAddress = "wuftuser@intel.com", FullName = "SYS DCULT" });
        }

        private void SeedWarehouses(WUFT.EF.WUFTDbContext context)
        {
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "CNA2", PrimaryEmailAddresses = "inventory.control.cna2@intel.com", CCEmailAddresses = "inventory.control.cn20@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "CNA8", PrimaryEmailAddresses = "customer.service.shanghai.cna8@intel.com", CCEmailAddresses = "david.yang@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "NLA3", PrimaryEmailAddresses = "ams.hvi@intel.com;ams.wm@intel.com;cedric.menke@intel.com;customer.service.amsterdam.helpdesk@intel.com;mehmet.ozel@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "TX03", PrimaryEmailAddresses = "customer.service.elpaso.tx03@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "VNA1", PrimaryEmailAddresses = "customer.service.vietnam.vna1.fg@intel.com", CCEmailAddresses = "marigold.ann.a.velarga@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "AZ04", PrimaryEmailAddresses = "customer.service.arizona.az04.az05@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "MYB1", PrimaryEmailAddresses = "customer.service.malaysia.myb1.mya5.myc1@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "NLA6", PrimaryEmailAddresses = "ams.hvi@intel.com;ams.wm@intel.com;cedric.menke@intel.com;customer.service.amsterdam.helpdesk@intel.com;mehmet.ozel@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "NLA4", PrimaryEmailAddresses = "ams.hvi@intel.com;ams.wm@intel.com;cedric.menke@intel.com;customer.service.amsterdam.helpdesk@intel.com;mehmet.ozel@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "CNA9", PrimaryEmailAddresses = "customer.service.shenzhen.cna9@intel.com", CCEmailAddresses = "david.yang@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "CNC5", PrimaryEmailAddresses = "customer.service.chengdu.cnc5@intel.com", CCEmailAddresses = "ranran.wang@intel.com;chunmeng.liu@intel.com;dongmeix.yang@intel.com;yux.huang@intel.com;longqiang.ran@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "MYA5", PrimaryEmailAddresses = "customer.service.malaysia.myb1.mya5.myc1@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "JPA4", PrimaryEmailAddresses = "customer.service.japan.jpa4@intel.com", CCEmailAddresses = "ryosukex.atsumi@intel.com;sonomi.seki@intel.com;yuukix.tanaka@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "CND4", PrimaryEmailAddresses = "customer.service.shanghai.cnd4@intel.com" });
            context.Warehouses.AddOrUpdate(new Warehouse { PlantCode = "DEA1", PrimaryEmailAddresses = "customer.service.vietnam.vna1.fg@intel.com" });
        }

        private void SeedErrorEmailCCs(WUFT.EF.WUFTDbContext context)
        {
            context.ErrorEmailCCs.AddOrUpdate(new ErrorEmailCC { EmailAddress = "grp_ask.ems@intel.com" });
            context.ErrorEmailCCs.AddOrUpdate(new ErrorEmailCC { EmailAddress = "benedict.zhang@intel.com" });
        }

        private void CreateStoredProcedures(WUFT.EF.WUFTDbContext context)
        {
            string sql = @"CREATE PROCEDURE [dbo].[uspLoadNewUnits]
(@QRELoadJobID VARCHAR(50)
, @QREUser VARCHAR(50)
, @MRBID INT
, @Merge BIT = 0)

AS
BEGIN
	SET NOCOUNT ON 

	-- ### SETUP STUFF ###

	DECLARE  @RequestGroup  TABLE( 
                                 RowID     INT    IDENTITY ( 1 , 1 ), 
                                 Warehouse VARCHAR(50), 
                                 [DispositionID] VARCHAR(50) 
                                 )

    CREATE TABLE #TempUnits (
	[FlagRequestID] [int] NOT NULL,
	[OriginalLotNumber] [varchar](20) NOT NULL,
	[DestinationLotNumber] [varchar](20) NOT NULL,
	[OriginalBoxNumber] [varchar](20) NOT NULL,
	[SubstrateVisualID] [varchar](64) NOT NULL,
	[UploadedWarehouseName] [nvarchar](max) NULL,
	[DispositionID] [int] NOT NULL,
	[StartDateTime] [datetime] NOT NULL,
	[EndDateTime] [datetime] NOT NULL,
	[MRBID] [int] NOT NULL,
	[FacilityId] [int] NOT NULL,
	[LabelQuantity] [int] NOT NULL,
	[SequenceNumber] [int] NOT NULL,
	[CarrierX] [int] NOT NULL,
	[CarrierY] [int] NOT NULL,
	[ScanDateTime] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [varchar](10) NULL,
	[LastUpdateOn] [datetime] NOT NULL,
	[LastUpdateBy] [varchar](10) NULL,
	[UnitFound] [bit])


	DECLARE @FlagRequestIDs TABLE (FlagRequestID INT)

	DECLARE @DuplicateVisualIDs TABLE (FlagRequestID INT,MRBID INT,SubstrateVisualID VARCHAR(64))

	DECLARE @VisualIDs TABLE (FlagRequestID INT,MRBID INT,SubstrateVisualID VARCHAR(64), BoxID VARCHAR(64), Disposition INT)

	DECLARE  @imax INT, 
             @i    INT,
			 @FlagRequestID INT

    DECLARE @ReleaseID INT
	SELECT @ReleaseID=[DispositionID] FROM [dbo].[Dispositions] WHERE [DispositionName] = 'Release'

	DECLARE @NewID INT
	SELECT @NewID=[RequestStatusID] FROM [dbo].[RequestStatus] WHERE [RequestStatusName] = 'New Request'

	DECLARE @InProcessID INT
	SELECT @InProcessID=[RequestStatusID] FROM [dbo].[RequestStatus] WHERE [RequestStatusName] = 'In Process'

	DECLARE @CompleteID INT
	SELECT @CompleteID=[RequestStatusID] FROM [dbo].[RequestStatus] WHERE [RequestStatusName] = 'Completed'

	DECLARE @ErrorCode INT
	SET @ErrorCode = 0

	INSERT INTO @RequestGroup (Warehouse,DispositionID)
	SELECT Warehouse, DispositionID
	FROM [dbo].[QRELoads]
	WHERE QRELoadJobID = @QRELoadJobID
	GROUP BY Warehouse, DispositionID

	SET @imax = @@ROWCOUNT
	SET @i = 1

	-- ### END SETUP STUFF ###

	-- ### FIND DUPLICATE BOX WITH SAME MRB ### ---

	DECLARE @VIDCOUNT INT

	INSERT INTO @VisualIDs (FlagRequestID,MRBID,SubstrateVisualID,BoxID,Disposition)
	SELECT DISTINCT previous.FlagRequestID,previous.MRBID,new.VisualID,previous.OriginalBoxNumber,new.DispositionID FROM [dbo].[QRELoads] new
	JOIN (SELECT DISTINCT fr.FlagRequestID,fr.MRBID,ecd.SubstrateVisualID,fr.RequestStatusID,fr.LastModifiedBy,fr.DispositionID,ecd.OriginalBoxNumber
			FROM [dbo].[FlagRequests] fr 
			JOIN [dbo].[ECD_WarehouseDemix] ecd
				ON fr.FlagRequestID = ecd.FlagRequestID) previous
	ON previous.OriginalBoxNumber = new.BoxID
	WHERE previous.RequestStatusID IN (@NewID)
		AND new.QRELoadJobID = @QRELoadJobID
		AND previous.MRBID = @MRBID
		AND new.DispositionID = previous.DispositionID

	SELECT @VIDCOUNT=COUNT(*) FROM @VisualIDs

	IF(@VIDCOUNT > 0 AND @Merge = 0)
	BEGIN
		SET @ErrorCode = 6000 -- DUPS FOUND WITH SAME MRB AND DISPOSITION
		GOTO FINISH
	END


	DECLARE @MERGEFlagRequestID INT
	IF(@VIDCOUNT > 0 AND @Merge = 1)
	BEGIN
		SELECT TOP 1  @MERGEFlagRequestID=FlagRequestID FROM @VisualIDs GROUP BY FlagRequestID
		GOTO COMPLETE_MERGE
	END


	-- ### END FIND DUPLICATE VIDs WITH SAME MRB ### ---

	-- ### FIND DUPLICATE VIDs FROM DIFFERENT QRE ###
	DECLARE @DUPLICATEVIDCOUNT INT

	INSERT INTO @DuplicateVisualIDs (FlagRequestID,MRBID,SubstrateVisualID)
	SELECT previous.FlagRequestID,previous.MRBID,new.VisualID FROM [dbo].[QRELoads] new
	JOIN (SELECT DISTINCT fr.FlagRequestID,fr.MRBID,ecd.SubstrateVisualID, fr.RequestStatusID, fr.LastModifiedBy
			FROM [dbo].[FlagRequests] fr
			JOIN [dbo].[ECD_WarehouseDemix] ecd
				ON fr.FlagRequestID = ecd.FlagRequestID) previous
	ON previous.SubstrateVisualID = new.VisualID
	WHERE previous.RequestStatusID IN (@NewID) 
		AND new.QRELoadJobID = @QRELoadJobID
		AND previous.MRBID != @MRBID
		--AND ((previous.MRBID != @MRBID AND new.DispositionID != @ReleaseID) 
		--OR (previous.MRBID != @MRBID AND new.DispositionID = @ReleaseID))

	SELECT @DUPLICATEVIDCOUNT=COUNT(*) FROM @DuplicateVisualIDs

	IF (@DUPLICATEVIDCOUNT > 0)
	BEGIN
		SET @ErrorCode = 5000 -- DUPS FOUND FROM DIFF USER
		GOTO FINISH
	END

	-- ### END FIND DUPLICATE VIDs FROM DIFFERENT QRE ###

	-- ### FIND DUPLICATE VIDs IN PROCESS (CONFLICT EMAIL) ###
	INSERT INTO @DuplicateVisualIDs (FlagRequestID,MRBID,SubstrateVisualID)
	SELECT previous.FlagRequestID,previous.MRBID,new.VisualID FROM [dbo].[QRELoads] new
	JOIN (SELECT DISTINCT fr.FlagRequestID,fr.MRBID,ecd.SubstrateVisualID, fr.RequestStatusID, fr.LastModifiedBy
			FROM [dbo].[FlagRequests] fr
			JOIN [dbo].[ECD_WarehouseDemix] ecd
				ON fr.FlagRequestID = ecd.FlagRequestID) previous
	ON previous.SubstrateVisualID = new.VisualID
	WHERE previous.RequestStatusID IN (@InProcessID) 
		AND new.QRELoadJobID = @QRELoadJobID

	SELECT @DUPLICATEVIDCOUNT=COUNT(*) FROM @DuplicateVisualIDs

	IF (@DUPLICATEVIDCOUNT > 0)
	BEGIN
		SET @ErrorCode = 5001 -- DUPS FOUND IN PROCESS
		GOTO FINISH
	END

	-- ### END FIND DUPLICATE VIDs IN PROCESS ###

	COMPLETE_MERGE:

	IF (@ErrorCode <= 0)
	BEGIN
		WHILE (@i <= @imax)
		BEGIN
		

			IF(@Merge = 0)
			BEGIN
			INSERT INTO [dbo].[FlagRequests](LastModified, LastModifiedBy, CreatedOn, CreatedBy, RequestStatusID, DispositionID, MRBID, WarehouseName)
			SELECT
				  GETUTCDATE()
				, @QREUser
				, GETUTCDATE()
				, @QREUser
				, CASE WHEN ql.DispositionID = @ReleaseID THEN @CompleteID ELSE @NewID END
				,ql.DispositionID
				, @MRBID
				, ql.Warehouse
			FROM [dbo].[QRELoads] ql
			JOIN @RequestGroup rg ON
				ql.Warehouse = rg.Warehouse AND
				ql.DispositionID = rg.DispositionID
			WHERE QRELoadJobID = @QRELoadJobID
					AND rg.RowID = @i
			GROUP BY ql.Warehouse,ql.DispositionID
				
			SET @FlagRequestID = @@IDENTITY

			INSERT INTO @FlagRequestIDs(FlagRequestID)
			SELECT @FlagRequestID
			END

			IF(@Merge = 1 AND @MERGEFlagRequestID > 1)
			BEGIN
				SET @FlagRequestID = @MERGEFlagRequestID
			END

			INSERT INTO #TempUnits (FlagRequestID, OriginalLotNumber, DestinationLotNumber, OriginalBoxNumber, SubstrateVisualID, UploadedWarehouseName
			, DispositionID, StartDateTime, EndDateTime, MRBID, FacilityId, LabelQuantity, SequenceNumber, CarrierX, CarrierY
			, ScanDateTime, CreatedOn, CreatedBy, LastUpdateOn, LastUpdateBy, UnitFound)
			SELECT 
				  @FlagRequestID FlagRequestID
				, LotID OriginalLotNumber
				, FPOLotID DestinationLotNumber
				, BoxID OriginalBoxNumber
				, VisualID SubstrateVisualID
				, ql.Warehouse UploadedWarehouseName
				, ql.DispositionID DispositionID
				, GETUTCDATE() StartDateTime
				, GETUTCDATE() EndDateTime
				, 0 MRBID
				, 0 FacilityId
				, 0 LabelQuantity
				, 0 SequenceNumber
				, 0 CarrierX
				, 0 CarrierY
				, '1/1/1900' ScanDateTime
				, GETUTCDATE() CreatedOn
				, @QREUser CreatedBy
				, GETUTCDATE() LastUpdateOn
				, @QREUser LastUpdateBy
				, 0 UnitFound
			FROM [dbo].[QRELoads] ql
			JOIN @RequestGroup rg ON
				ql.Warehouse = rg.Warehouse AND
				ql.DispositionID = rg.DispositionID
			WHERE QRELoadJobID = @QRELoadJobID
					AND rg.RowID = @i

			SET @i = @i + 1
		END

		MERGE [dbo].[ECD_WarehouseDemix] AS Target
		USING #TempUnits AS Source
		ON (Target.SubstrateVisualID = Source.SubstrateVisualID
		AND Target.MRBID = Source.MRBID)
		WHEN MATCHED THEN
			UPDATE SET Target.FlagRequestID = Source.FlagRequestID,
			Target.OriginalLotNumber = Source.OriginalLotNumber,
			Target.DestinationLotNumber = Source.DestinationLotNumber, 
			Target.OriginalBoxNumber = Source.OriginalBoxNumber,
			Target.UploadedWarehouseName = Source.UploadedWarehouseName,
			Target.StartDateTime = Source.StartDateTime,
			Target.MRBID = Source.MRBID,
			Target.FacilityId = Source.FacilityId,
			Target.DispositionID = Source.DispositionID,
			Target.LabelQuantity = Source.LabelQuantity,
			Target.SequenceNumber = Source.SequenceNumber,
			Target.CarrierX = Source.CarrierX,
			Target.CarrierY = Source.CarrierY,
			Target.ScanDateTime = Source.ScanDateTime,
			--Target.CreatedOn = Source.CreatedOn,
			--Target.CreatedBy = Source.CreatedBy,
			Target.LastUpdateOn = Source.LastUpdateOn,
			Target.LastUpdateBy = Source.LastUpdateBy,
			Target.UnitFound = Source.UnitFound
		WHEN NOT MATCHED BY TARGET THEN
			INSERT (FlagRequestID, OriginalLotNumber, DestinationLotNumber, OriginalBoxNumber, SubstrateVisualID, UploadedWarehouseName
			, DispositionID, StartDateTime, EndDateTime, MRBID, FacilityId, LabelQuantity, SequenceNumber, CarrierX, CarrierY
			, ScanDateTime, CreatedOn, CreatedBy, LastUpdateOn, LastUpdateBy, UnitFound)
			VALUES (Source.FlagRequestID, Source.OriginalLotNumber, Source.DestinationLotNumber, Source.OriginalBoxNumber, Source.SubstrateVisualID, Source.UploadedWarehouseName
			, Source.DispositionID, Source.StartDateTime, Source.EndDateTime, Source.MRBID, Source.FacilityId, Source.LabelQuantity
			, Source.SequenceNumber, Source.CarrierX, Source.CarrierY, Source.ScanDateTime, Source.CreatedOn, Source.CreatedBy, Source.LastUpdateOn
			, Source.LastUpdateBy
			, Source.UnitFound);

	END
		

	FINISH:

	IF @ErrorCode = 6000
	BEGIN
		SELECT @ErrorCode ErrorCode
		SELECT FlagRequestID,MRBID,SubstrateVisualID,BoxID,Disposition from @VisualIDs
	END
	
	IF @ErrorCode = 5000 OR @ErrorCode = 5001
	BEGIN
		DELETE FROM QRELoads WHERE QRELoadJobID = @QRELoadJobID
		SELECT @ErrorCode ErrorCode
		SELECT FlagRequestID,MRBID,SubstrateVisualID FROM @DuplicateVisualIDs
	END
	
	IF @ErrorCode = 0
	BEGIN
		DELETE FROM QRELoads WHERE QRELoadJobID = @QRELoadJobID
		SELECT @ErrorCode ErrorCode
		SELECT FlagRequestID FROM @FlagRequestIDs
	END
END




GO";
            context.Database.ExecuteSqlCommand(sql);
        }
    }
}
