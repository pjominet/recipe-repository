namespace RecipeRepository.Data.Entities.Nomenclature;

public class Tag
{
    public int Id { get; set; }
    public int TagCategoryId { get; set; }
    public string Label { get; set; }

    public virtual ICollection<RecipeTags> RecipeTags { get; set; } = new HashSet<RecipeTags>();
    public virtual TagCategory TagCategory { get; set; }
}
