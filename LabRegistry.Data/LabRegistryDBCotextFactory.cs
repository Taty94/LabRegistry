using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LabRegistry.Data;

public class LabRegistryDBCotextFactory : IDesignTimeDbContextFactory<LabRegistryDbContext>
{
    public LabRegistryDbContext CreateDbContext(string[] args)
    {
        string connectionString ="Host=localhost;Port=5432;Database=lab_registry;Username=postgres;Password=admin";
        var optionsBuilder = new DbContextOptionsBuilder<LabRegistryDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new LabRegistryDbContext(optionsBuilder.Options);
    }
}
