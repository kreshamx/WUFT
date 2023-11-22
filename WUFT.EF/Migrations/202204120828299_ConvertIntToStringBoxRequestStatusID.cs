namespace WUFT.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConvertIntToStringBoxRequestStatusID : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.BoxRequestStatus");
            AlterColumn("dbo.BoxRequestStatus", "BoxRequestStatusID", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.BoxRequestStatus", "BoxRequestStatusID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.BoxRequestStatus");
            AlterColumn("dbo.BoxRequestStatus", "BoxRequestStatusID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.BoxRequestStatus", "BoxRequestStatusID");
        }
    }
}
