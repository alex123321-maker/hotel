namespace hotel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetDefaultForComplited : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationServiceModels", "Complited", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationServiceModels", "Complited");
        }
    }
}
