namespace RecipeRepository.Logic.Infrastructure.Extensions;

public static class StringExtensions
{
    public static bool HasValue(this string? @string, bool checkWhitespace = true)
    {
        return checkWhitespace
            ? !string.IsNullOrWhiteSpace(@string)
            : !string.IsNullOrEmpty(@string);
    }
}
