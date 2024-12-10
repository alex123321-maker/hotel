using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace hotel.Models
{
    public class ModelsContext:DbContext
    {

        public ModelsContext() : base("DefaultConnection")
        {
        }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<DiscountModel> Discounts { get; set; }
        public DbSet<PricingModel> Pricings { get; set; }
        public DbSet<ReservationModel> Reservations { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<RoomTypeModel> RoomTypes { get; set; }
        public DbSet<RoomCapacityModel> RoomCapacities { get; set; }
        public DbSet<StatusModel> Statuses { get; set; }
        public DbSet<ServiceModel> Services { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<ReservationServiceModel> ReservationServices { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Настройка связей многие-ко-многим между ReservationModel и ServiceModel
            modelBuilder.Entity<ReservationModel>()
                .HasMany(r => r.ReservationServices)
                .WithRequired(rs => rs.Reservation)
                .HasForeignKey(rs => rs.ReservationId);

            modelBuilder.Entity<ServiceModel>()
                .HasMany(s => s.ReservationServices)
                .WithRequired(rs => rs.Service)
                .HasForeignKey(rs => rs.ServiceId);

            modelBuilder.Entity<ReservationModel>()
                .HasRequired(r => r.Admin) 
                .WithMany() 
                .HasForeignKey(r => r.AdminId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }


    }
}