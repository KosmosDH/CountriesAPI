using CountriesAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace CountryAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Alpha2Code).IsUnique();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShortName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Alpha2Code).IsRequired().HasMaxLength(2);
        });
    }
}