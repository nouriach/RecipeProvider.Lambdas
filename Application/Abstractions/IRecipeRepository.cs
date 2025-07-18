using RecipeProvider.Lambdas.Persistence.DTOs;

namespace RecipeProvider.Lambdas.Application.Abstractions;

public interface IRecipeRepository
{
    Task<List<RecipeDto>> GetRandomRecipesByBook(string book);
    Task<RecipeDto?> GetRecipeByPrimaryKey(string recipeId);
}