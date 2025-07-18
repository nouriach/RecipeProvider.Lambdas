using RecipeProvider.Lambdas.Application.Abstractions;
using RecipeProvider.Lambdas.Application.Models;

namespace RecipeProvider.Lambdas.Application.Services;

public class RecipeService(IRecipeRepository recipeRepository) : IRecipeService
{
    public async Task<List<Recipe>> GetRandomRecipesByBook(string book)
    {
        var recipeDtos = await recipeRepository.GetRandomRecipesByBook(book);

        return recipeDtos.Select(r => new Recipe
        {
            Title = r.Title,
            Summary = new RecipeSummary
            {
                PrepTime = r.PrepTime,
                Serves = r.Serves,
                CookTime = r.CookTime
            },
            // Ingredients = r.Ingredients,
            // Book = r.Book
        }).ToList();
    }

    public Task<List<Recipe>> GetRecipeById(string recipeId)
    {
        throw new NotImplementedException();
    }
}