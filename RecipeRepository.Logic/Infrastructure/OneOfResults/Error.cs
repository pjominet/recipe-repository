namespace RecipeRepository.Logic.Infrastructure.OneOfResults;

public class Error(string message) : MessageResult(message);
