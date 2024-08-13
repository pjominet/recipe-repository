using RRContext = RecipeRepository.Data.Contexts.RRContext;

namespace RecipeRepository.Data.Repositories;

public class UserRepository(RRContext context) : BaseRepository<RRContext>(context);
