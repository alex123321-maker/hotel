namespace hotel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixRoomType : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RoomTypeModels", "Price");
            DropColumn("dbo.RoomTypeModels", "Capacity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RoomTypeModels", "Capacity", c => c.Int(nullable: false));
            AddColumn("dbo.RoomTypeModels", "Price", c => c.Int(nullable: false));
        }
    }
}
