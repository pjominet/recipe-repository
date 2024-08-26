namespace RecipeRepository.Logic.Infrastructure.OneOfResults;

public abstract class MessageResult(string message)
{
    public string Message { get; } = message;
}
