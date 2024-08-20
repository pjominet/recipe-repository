using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Entities.Identity;

namespace RecipeRepository.Data.Contexts;

public partial class RecipeRepoContext : IdentityDbContext<AppUser>
{
    public RecipeRepoContext() { }

    public RecipeRepoContext(DbContextOptions<RecipeRepoContext> options) : base(options) { }

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
