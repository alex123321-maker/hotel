namespace hotel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Surname = c.String(),
                        Lastname = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Passportseries = c.Int(nullable: false),
                        Passportnumber = c.Int(nullable: false),
                        Prefers = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DiscountModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Days = c.Int(nullable: false),
                        Amount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PaymentModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReservationId = c.Int(nullable: false),
                        PaymentDate = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentMethod = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ReservationModels", t => t.ReservationId, cascadeDelete: true)
                .Index(t => t.ReservationId);
            
            CreateTable(
                "dbo.ReservationModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        RoomId = c.Int(nullable: false),
                        AdminId = c.String(nullable: false, maxLength: 128),
                        DiscountId = c.Int(nullable: false),
                        StatusId = c.String(),
                        CheckInDate = c.DateTime(nullable: false),
                        CheckOutDate = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(),
                        Status_Title = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AdminId)
                .ForeignKey("dbo.CustomerModels", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.DiscountModels", t => t.DiscountId, cascadeDelete: true)
                .ForeignKey("dbo.RoomModels", t => t.RoomId, cascadeDelete: true)
                .ForeignKey("dbo.StatusModels", t => t.Status_Title)
                .Index(t => t.CustomerId)
                .Index(t => t.RoomId)
                .Index(t => t.AdminId)
                .Index(t => t.DiscountId)
                .Index(t => t.Status_Title);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ReservationServiceModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReservationId = c.Int(nullable: false),
                        ServiceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ServiceModels", t => t.ServiceId, cascadeDelete: true)
                .ForeignKey("dbo.ReservationModels", t => t.ReservationId, cascadeDelete: true)
                .Index(t => t.ReservationId)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.ServiceModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Price = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoomModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoomTypeId = c.Int(nullable: false),
                        CapacityId = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RoomCapacityModels", t => t.CapacityId, cascadeDelete: true)
                .ForeignKey("dbo.RoomTypeModels", t => t.RoomTypeId, cascadeDelete: true)
                .Index(t => t.RoomTypeId)
                .Index(t => t.CapacityId);
            
            CreateTable(
                "dbo.RoomCapacityModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Capacity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoomTypeModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        Price = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StatusModels",
                c => new
                    {
                        Title = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Title);
            
            CreateTable(
                "dbo.PricingModels",
                c => new
                    {
                        day_of_week = c.String(nullable: false, maxLength: 128),
                        multiplier = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.day_of_week);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PaymentModels", "ReservationId", "dbo.ReservationModels");
            DropForeignKey("dbo.ReservationModels", "Status_Title", "dbo.StatusModels");
            DropForeignKey("dbo.ReservationModels", "RoomId", "dbo.RoomModels");
            DropForeignKey("dbo.RoomModels", "RoomTypeId", "dbo.RoomTypeModels");
            DropForeignKey("dbo.RoomModels", "CapacityId", "dbo.RoomCapacityModels");
            DropForeignKey("dbo.ReservationServiceModels", "ReservationId", "dbo.ReservationModels");
            DropForeignKey("dbo.ReservationServiceModels", "ServiceId", "dbo.ServiceModels");
            DropForeignKey("dbo.ReservationModels", "DiscountId", "dbo.DiscountModels");
            DropForeignKey("dbo.ReservationModels", "CustomerId", "dbo.CustomerModels");
            DropForeignKey("dbo.ReservationModels", "AdminId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.RoomModels", new[] { "CapacityId" });
            DropIndex("dbo.RoomModels", new[] { "RoomTypeId" });
            DropIndex("dbo.ReservationServiceModels", new[] { "ServiceId" });
            DropIndex("dbo.ReservationServiceModels", new[] { "ReservationId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.ReservationModels", new[] { "Status_Title" });
            DropIndex("dbo.ReservationModels", new[] { "DiscountId" });
            DropIndex("dbo.ReservationModels", new[] { "AdminId" });
            DropIndex("dbo.ReservationModels", new[] { "RoomId" });
            DropIndex("dbo.ReservationModels", new[] { "CustomerId" });
            DropIndex("dbo.PaymentModels", new[] { "ReservationId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.PricingModels");
            DropTable("dbo.StatusModels");
            DropTable("dbo.RoomTypeModels");
            DropTable("dbo.RoomCapacityModels");
            DropTable("dbo.RoomModels");
            DropTable("dbo.ServiceModels");
            DropTable("dbo.ReservationServiceModels");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ReservationModels");
            DropTable("dbo.PaymentModels");
            DropTable("dbo.DiscountModels");
            DropTable("dbo.CustomerModels");
        }
    }
}
