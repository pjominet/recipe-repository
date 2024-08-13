namespace RecipeRepository.Data.Entities.Nomenclature;

public class TagCategory
{
    public int Id { get; set; }
    public string Label { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
}
