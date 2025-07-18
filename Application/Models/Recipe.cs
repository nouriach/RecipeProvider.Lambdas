namespace RecipeProvider.Lambdas.Application.Models;

public class Recipe
{
    public string Title { get; set; } = string.Empty;
    public RecipeSummary Summary { get; set; } = null!;
    public List<string> Ingredients { get; set; } = new();
    public string Book { get; set; }
    public string FilePath { get; set; } = string.Empty;
}