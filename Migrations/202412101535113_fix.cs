namespace hotel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ReservationModels", "StatusId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReservationModels", "StatusId", c => c.String());
        }
    }
}
