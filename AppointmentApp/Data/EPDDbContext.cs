using Microsoft.EntityFrameworkCore;
using Chipsoft.Assignments.EPDConsole.Domain;

namespace Chipsoft.Assignments.EPDConsole.Data
{
    // EF Core DbContext: sqlite file "epd.db" als default
    public class EPDDbContext : DbContext
    {
        public EPDDbContext() { }
        public EPDDbContext(DbContextOptions<EPDDbContext> options) : base(options) { }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Physician> Physicians => Set<Physician>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
                options.UseSqlite("Data Source=epd.db");
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            // Patient: uniek rijksregisternummer
            b.Entity<Patient>()
                .HasIndex(p => p.NationalNumber).IsUnique();
            b.Entity<Patient>()
                .Property(p => p.NationalNumber).IsRequired();

            // Physician: uniek registratienummer
            b.Entity<Physician>()
                .HasIndex(d => d.RegistrationNumber).IsUnique();
            b.Entity<Physician>()
                .Property(d => d.RegistrationNumber).IsRequired();

            // Appointment relaties
            b.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Appointment>()
                .HasOne(a => a.Physician)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.PhysicianId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Appointment>()
                .HasIndex(a => new { a.PhysicianId, a.StartsAt, a.EndsAt });
        }
    }
}
