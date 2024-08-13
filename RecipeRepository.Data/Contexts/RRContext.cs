using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Entities.Identity;

namespace RecipeRepository.Data.Contexts;

public partial class RRContext : IdentityDbContext<AppUser>
{
    public RRContext() { }

    public RRContext(DbContextOptions<RRContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingBase(modelBuilder);
        OnModelCreatingNomenclature(modelBuilder);
    }
}
