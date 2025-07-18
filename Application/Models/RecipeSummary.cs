namespace RecipeProvider.Lambdas.Application.Models;

public class RecipeSummary
{
    public int Serves { get; set; }
    public string PrepTime { get; set; } = string.Empty;
    public string CookTime { get; set; } = string.Empty;
}