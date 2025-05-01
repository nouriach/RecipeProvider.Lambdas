namespace RecipeProvider.Lambdas.Config;

public class DynamoDbSettings
{
    public const string SectionName = "DynamoDbTables";
    public string RecipesTable { get; set; } = String.Empty;   
}