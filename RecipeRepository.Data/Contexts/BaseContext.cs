using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Entities;

namespace RecipeRepository.Data.Contexts;

// ReSharper disable once InconsistentNaming
public partial class RRContext
{
    public virtual DbSet<Ingredient> Ingredients { get; set; }
    public virtual DbSet<Recipe> Recipes { get; set; }
    public virtual DbSet<RecipeLikes> RecipeLikes { get; set; }
    public virtual DbSet<RecipeTags> RecipeTagAssociations { get; set; }

    private void OnModelCreatingBase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.ToTable("Ingredient", "rr");

            entity.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(32);

            entity.HasOne(i => i.QuantityUnit)
                .WithMany(q => q.Ingredients)
                .HasForeignKey(i => i.QuantityUnitId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Ingredient_Quantity");

            entity.HasOne(i => i.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(i => i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Ingredient_Recipe");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.ToTable("Recipe", "rr");

            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(r => r.Description)
                .HasMaxLength(512);

            entity.Property(r => r.ImageUri)
                .HasMaxLength(4000);

            entity.Property(r => r.OriginalImageName)
                .HasMaxLength(128);

            entity.Property(r => r.Preparation)
                .HasMaxLength(4000)
                .IsRequired();

            entity.Property(r => r.CreatedOn).HasColumnType("datetime2");

            entity.Property(r => r.UpdatedOn).HasColumnType("datetime2");

            entity.Property(r => r.DeletedOn).HasColumnType("datetime2");

            entity.HasOne(r => r.Cost)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CostId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Recipe_Cost");

            entity.HasOne(r => r.Difficulty)
                .WithMany(d => d.Recipes)
                .HasForeignKey(r => r.DifficultyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Recipe_Difficulty");

            entity.HasOne(r => r.User)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recipe_User");
        });

        modelBuilder.Entity<RecipeLikes>(entity =>
        {
            entity.HasKey(rta => new {rta.RecipeId, rta.UserId});

            entity.ToTable("RecipeLikes", "rr");

            entity.HasOne(rl => rl.Recipe)
                .WithMany(r => r.RecipeLikes)
                .HasForeignKey(rl => rl.RecipeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_RecipeLike_Recipe");

            entity.HasOne(rl => rl.User)
                .WithMany(u => u.RecipeLikes)
                .HasForeignKey(rl => rl.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_RecipeLike_User");
        });

        modelBuilder.Entity<RecipeTags>(entity =>
        {
            entity.HasKey(rta => new {rta.RecipeId, rta.TagId});

            entity.ToTable("RecipeTags", "rr");

            entity.HasOne(rta => rta.Recipe)
                .WithMany(r => r.RecipeTags)
                .HasForeignKey(rta => rta.RecipeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_RecipeTag_Recipe");

            entity.HasOne(rta => rta.Tag)
                .WithMany(t => t.RecipeTagAssociations)
                .HasForeignKey(rta => rta.TagId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_RecipeTag_Tag");
        });
    }
}
