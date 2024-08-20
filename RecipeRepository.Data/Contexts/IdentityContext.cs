using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Entities.Identity;

namespace RecipeRepository.Data.Contexts;

public partial class RecipeRepoContext
{
    public DbSet<AppUser> AppUsers { get; set; }

    private void OnModelCreatingIdentity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("AppUser", "identity");

            entity.Property(e => e.FirstName).HasMaxLength(64);

            entity.Property(e => e.LastName).HasMaxLength(64);

            entity.Property(e => e.ProfileImageUri).HasMaxLength(4000);

            entity.Property(e => e.CreatedOn)
                .HasDefaultValue(DateTime.UtcNow)
                .HasColumnType("datetime2");

            entity.Property(e => e.UpdatedOn).HasColumnType("datetime2");

            entity.Property(e => e.DeletedOn).HasColumnType("datetime2");
        });
    }
}
