using RecipeProvider.Lambdas.Application.Models;

namespace RecipeProvider.Lambdas.Application.Abstractions;

public interface IEmailClient
{
    Task SendEmail(List<Recipe> emailContent);
}