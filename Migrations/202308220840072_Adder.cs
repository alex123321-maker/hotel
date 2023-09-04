namespace hotel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationModels", "CreatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationModels", "CreatedDate");
        }
    }
}
