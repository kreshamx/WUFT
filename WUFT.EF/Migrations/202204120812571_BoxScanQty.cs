namespace WUFT.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BoxScanQty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoxRequestStatus", "BoxScanQty", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoxRequestStatus", "BoxScanQty");
        }
    }
}
