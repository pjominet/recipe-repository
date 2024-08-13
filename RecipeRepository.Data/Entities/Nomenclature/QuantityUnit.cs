namespace RecipeRepository.Data.Entities.Nomenclature
{
    public class QuantityUnit
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Ingredient> Ingredients { get; set; } = new HashSet<Ingredient>();
    }
}
