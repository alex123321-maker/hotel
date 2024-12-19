namespace hotel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ReservationModels", "DiscountId", "dbo.DiscountModels");
            DropIndex("dbo.ReservationModels", new[] { "DiscountId" });
            AlterColumn("dbo.ReservationModels", "DiscountId", c => c.Int());
            CreateIndex("dbo.ReservationModels", "DiscountId");
            AddForeignKey("dbo.ReservationModels", "DiscountId", "dbo.DiscountModels", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReservationModels", "DiscountId", "dbo.DiscountModels");
            DropIndex("dbo.ReservationModels", new[] { "DiscountId" });
            AlterColumn("dbo.ReservationModels", "DiscountId", c => c.Int(nullable: false));
            CreateIndex("dbo.ReservationModels", "DiscountId");
            AddForeignKey("dbo.ReservationModels", "DiscountId", "dbo.DiscountModels", "Id", cascadeDelete: true);
        }
    }
}
