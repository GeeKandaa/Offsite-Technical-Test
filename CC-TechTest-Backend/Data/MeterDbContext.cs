using CC_TechTest_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace CC_TechTest_Backend.Data
{
    public class MeterDbContext : DbContext
    {
        public MeterDbContext(DbContextOptions<MeterDbContext> options) : base(options) { }

        public DbSet<RowData> RegisteredMeters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RowData>()
                .HasKey(m => new { m.MPAN, m.DateOfInstallation, m.MeterSerial });

            modelBuilder.Entity<RowData>()
                .Property(m => m.MPAN)
                .HasColumnType("numeric(13,0)")
                .IsRequired();

            modelBuilder.Entity<RowData>()
                .Property(m => m.MeterSerial)
                .HasMaxLength(10)
                .IsRequired();

            modelBuilder.Entity<RowData>()
                .Property(m => m.DateOfInstallation)
                .HasColumnType("date")
                .IsRequired();

            modelBuilder.Entity<RowData>()
                .Property(m => m.AddressLine1)
                .HasMaxLength(40);

            modelBuilder.Entity<RowData>()
                .Property(m => m.Postcode)
                .HasMaxLength(10);

            modelBuilder.Entity<RowData>().ToTable(t => t.HasCheckConstraint("CK_DATE_IS_PAST", @"[DateOfInstallation]<CONVERT([date],getdate())"));
            modelBuilder.Entity<RowData>().ToTable(t => t.HasCheckConstraint("CK_MPAN_FULL_LENGTH", @"len(CONVERT([varchar],[MPAN]))=(13)"));
            modelBuilder.Entity<RowData>().ToTable(t => t.HasCheckConstraint("CK_SERIAL_LENGTH", @"len([MeterSerial])>=(1) AND len([MeterSerial])<=(10)"));
            modelBuilder.Entity<RowData>().ToTable(t => t.HasCheckConstraint("CK_POSTCODE_FORMAT", @"[PostCode] IS NULL OR [PostCode] like '[A-Z][A-Z][0-9] [0-9][A-Z][A-Z]'"));
        }
    }
}
