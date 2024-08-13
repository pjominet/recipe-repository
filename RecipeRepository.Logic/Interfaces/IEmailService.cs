namespace RecipeRepository.Logic.Interfaces;

public interface IEmailService
{
    public Task SendEmailAsync(string to, string subject, string html, string? sender = null);
}
