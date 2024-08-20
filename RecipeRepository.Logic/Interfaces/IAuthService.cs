namespace RecipeRepository.Logic.Interfaces;

public interface IAuthService
{
    public string GenerateToken(string username);
    public Task<bool> CheckCredentials(string username, string password);
}
