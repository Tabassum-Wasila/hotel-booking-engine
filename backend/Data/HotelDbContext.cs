using Microsoft.EntityFrameworkCore;
using HotelBookingEngine.Models;

namespace HotelBookingEngine.Data;

public class HotelDbContext(DbContextOptions<HotelDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<RatePlan> RatePlans => Set<RatePlan>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Timezone).HasMaxLength(50);
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.BaseRate).HasPrecision(10, 2);
            entity.Property(e => e.Amenities).HasColumnType("json");
            entity.Property(e => e.Photos).HasColumnType("json");

            entity.HasOne(e => e.Property)
                  .WithMany()
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date).HasColumnType("date");

            // Unique index: one inventory row per room type per date
            entity.HasIndex(e => new { e.RoomTypeId, e.Date }).IsUnique();

            entity.HasOne(e => e.RoomType)
                  .WithMany()
                  .HasForeignKey(e => e.RoomTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RatePlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RatePerNight).HasPrecision(10, 2);
            entity.Property(e => e.MealPlan).HasMaxLength(50);
            entity.Property(e => e.ValidFrom).HasColumnType("date");
            entity.Property(e => e.ValidTo).HasColumnType("date");

            entity.HasOne(e => e.RoomType)
                  .WithMany()
                  .HasForeignKey(e => e.RoomTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reference).IsRequired().HasMaxLength(8);
            entity.Property(e => e.GuestName).HasMaxLength(200);
            entity.Property(e => e.GuestEmail).HasMaxLength(255);
            entity.Property(e => e.GuestPhone).HasMaxLength(50);
            entity.Property(e => e.SpecialRequests).HasMaxLength(1000);
            entity.Property(e => e.NightlyRate).HasPrecision(10, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(10, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.CancellationRef).HasMaxLength(8);
            entity.Property(e => e.CheckIn).HasColumnType("date");
            entity.Property(e => e.CheckOut).HasColumnType("date");

            // Unique index on reference
            entity.HasIndex(e => e.Reference).IsUnique();
            // Index for guest lookup
            entity.HasIndex(e => new { e.GuestEmail, e.Status });
            // Index for reports
            entity.HasIndex(e => new { e.PropertyId, e.CheckIn, e.Status });

            entity.HasOne(e => e.Property)
                  .WithMany()
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RoomType)
                  .WithMany()
                  .HasForeignKey(e => e.RoomTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RatePlan)
                  .WithMany()
                  .HasForeignKey(e => e.RatePlanId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);

            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.Role)
                  .WithMany()
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}