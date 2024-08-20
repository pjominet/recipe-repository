using Microsoft.Extensions.Options;
using RecipeRepository.Logic.Infrastructure.Settings;
using RecipeRepository.Logic.Interfaces;

namespace RecipeRepository.Logic.Services;

public class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task SendEmailAsync(string to, string subject, string html, string? sender = null)
    {
        try
        {
            // create message
            await Task.CompletedTask;
            // TODO: find email provider
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }
}
