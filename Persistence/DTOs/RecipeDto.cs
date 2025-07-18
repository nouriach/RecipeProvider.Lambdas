using Amazon.DynamoDBv2.DataModel;

namespace RecipeProvider.Lambdas.Persistence.DTOs;

[DynamoDBTable("Recipes")]
public class RecipeDto
{
    [DynamoDBHashKey("recipeId")] // Primary key for the table
    public string RecipeId { get; set; } = Guid.NewGuid().ToString();

    [DynamoDBRangeKey] // Sort Key
    public string RecommendationKey { get; set; } = string.Empty;

    [DynamoDBProperty] // Regular attribute
    public string Title { get; set; } = string.Empty;

    [DynamoDBProperty] // Regular attribute
    public int Serves { get; set; }

    [DynamoDBProperty] // Regular attribute
    public string PrepTime { get; set; } = string.Empty;

    [DynamoDBProperty] // Regular attribute
    public string CookTime { get; set; } = string.Empty;

    [DynamoDBProperty] // Regular attribute
    public List<string> Ingredients { get; set; } = new();

    [DynamoDBProperty] // Regular attribute
    public int RecommendationCount { get; set; } = 0;

    [DynamoDBProperty] // Regular attribute
    public DateTime CreatedAt { get; set; }

    [DynamoDBProperty] // Regular attribute
    public DateTime? LastRecommendedAt { get; set; }

    [DynamoDBGlobalSecondaryIndexHashKey("Book-LastRecommendedAt-index")] // GSI Partition Key
    public string Book { get; set; } = string.Empty;

    [DynamoDBGlobalSecondaryIndexRangeKey("Book-LastRecommendedAt-index")] // GSI Sort Key
    public DateTime? LastRecommendedAtForIndex => LastRecommendedAt;
    
    public void EncodeRecommendationKey()
    {
        RecommendationKey = $"{RecommendationCount:D4}#{LastRecommendedAt:yyyy-MM-dd}";
    }
}