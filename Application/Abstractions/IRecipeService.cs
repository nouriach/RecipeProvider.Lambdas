using RecipeProvider.Lambdas.Application.Models;

namespace RecipeProvider.Lambdas.Application.Abstractions;

public interface IRecipeService
{
    Task<List<Recipe>> GetRandomRecipesByBook(string book);
    Task<List<Recipe>> GetRecipeById(string recipeId);
}