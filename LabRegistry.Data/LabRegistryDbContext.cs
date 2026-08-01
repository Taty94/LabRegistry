using LabRegistry.Models;
using Microsoft.EntityFrameworkCore;

namespace LabRegistry.Data;

public class LabRegistryDbContext : DbContext
{
    public LabRegistryDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Equipment> Equipment { get; set; } = null!;
    public DbSet<Student> Student { get; set; } = null!;
    public DbSet<Loan> Loan { get; set; } = null!;

  

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Evitar equipos con el mismo nombre
        modelBuilder.Entity<Equipment>()
            .HasIndex(e => e.Name)
            .IsUnique();

        // Evitar estudiantes con el mismo correo
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Email)
            .IsUnique();

        // Relación préstamo -> estudiante
        modelBuilder.Entity<Loan>()
            .HasOne<Student>()
            .WithMany()
            .HasForeignKey(l => l.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación préstamo -> equipo
        modelBuilder.Entity<Loan>()
            .HasOne<Equipment>()
            .WithMany()
            .HasForeignKey(r => r.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
