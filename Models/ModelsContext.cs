using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace hotel.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
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

            // Определение ключей для IdentityUserRole
            modelBuilder.Entity<IdentityUserRole>()
                .HasKey(r => new { r.UserId, r.RoleId })
                .ToTable("AspNetUserRoles");

            // Определение ключей для IdentityUserLogin
            modelBuilder.Entity<IdentityUserLogin>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
                .ToTable("AspNetUserLogins");

            // Убедитесь, что другие таблицы Identity настроены
            modelBuilder.Entity<IdentityUserClaim>()
                .ToTable("AspNetUserClaims");

            modelBuilder.Entity<IdentityRole>()
                .ToTable("AspNetRoles");

            base.OnModelCreating(modelBuilder);


        }

    }
}