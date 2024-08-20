using Microsoft.EntityFrameworkCore;
using RecipeRepository.Data.Entities.Nomenclature;

namespace RecipeRepository.Data.Contexts;

public partial class RecipeRepoContext
{
    public virtual DbSet<Cost> Costs { get; set; }
    public virtual DbSet<Difficulty> Difficulties { get; set; }
    public virtual DbSet<QuantityUnit> QuantityUnits { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<TagCategory> TagCategories { get; set; }

    private void OnModelCreatingNomenclature(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cost>(entity =>
        {
            entity.ToTable("Cost", "nomenclature");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(c => c.Label)
                .IsRequired()
                .HasMaxLength(32);
        });

        modelBuilder.Entity<Difficulty>(entity =>
        {
            entity.ToTable("Difficulty", "nomenclature");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(d => d.Label)
                .IsRequired()
                .HasMaxLength(32);
        });

        modelBuilder.Entity<QuantityUnit>(entity =>
        {
            entity.ToTable("QuantityUnit", "nomenclature");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(q => q.Label)
                .IsRequired()
                .HasMaxLength(32);

            entity.Property(q => q.Description).HasMaxLength(64);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag", "nomenclature");

            entity.Property(t => t.Label)
                .IsRequired()
                .HasMaxLength(32);

            entity.HasOne(t => t.TagCategory)
                .WithMany(tc => tc.Tags)
                .HasForeignKey(t => t.TagCategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Tag_TagCategory");
        });

        modelBuilder.Entity<TagCategory>(entity =>
        {
            entity.ToTable("TagCategory", "nomenclature");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(tc => tc.Label)
                .IsRequired()
                .HasMaxLength(32);
        });
    }
}
