namespace RecipeRepository.Logic.Infrastructure.OneOfResults;

public class BadRequest(string message) : MessageResult(message);
