using Microsoft.EntityFrameworkCore;
using Patient_Service.Models;

namespace Patient_Service.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().ToTable("Patient");
        modelBuilder.Entity<Organization>().ToTable("Organization");
    }

}