using Apbd11.Models;
using Microsoft.EntityFrameworkCore;

namespace Apbd11.Data;

public class PrescriptionContext : DbContext
{
    public PrescriptionContext(DbContextOptions<PrescriptionContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>().HasKey(d => d.IdDoctor);
        modelBuilder.Entity<Patient>().HasKey(p => p.IdPatient);
        modelBuilder.Entity<Medicament>().HasKey(m => m.IdMedicament);
        modelBuilder.Entity<Prescription>().HasKey(p => p.IdPrescription);

        modelBuilder.Entity<PrescriptionMedicament>()
            .HasKey(pm => new { pm.IdPrescription, pm.IdMedicament });
        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Prescription)
            .WithMany(p => p.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdPrescription);
        modelBuilder.Entity<PrescriptionMedicament>()
            .HasOne(pm => pm.Medicament)
            .WithMany(m => m.PrescriptionMedicaments)
            .HasForeignKey(pm => pm.IdMedicament);
    }

}
