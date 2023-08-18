using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace hotel.Models
{
    public class ModelsContext:DbContext
    {
        public DbSet<CustomerModels> Customers { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<RoomTypeModel> RoomTypes { get; set; }

        public DbSet<PricingModel> Pricings { get; set; }

        public DbSet<ServiceModel> Services{ get; set; }
        public DbSet<DiscountModel> Discounts{ get; set; }
        public DbSet<StatusModel> Statuses { get; set; }
        public DbSet<ReservationModel> Reservations { get; set; }


    }
}