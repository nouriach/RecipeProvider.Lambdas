using Amazon.DynamoDBv2;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeProvider.Lambdas.Application.Abstractions;
using RecipeProvider.Lambdas.Extensions;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace RecipeProvider.Lambdas;

public class Function
{
    private IRecipeService _recipeService;
    private IEmailClient _emailClient;

    public Function()
    {
        Console.WriteLine("---> About to call ConfigureServices");
        var serviceCollection = new ServiceCollection();
        ServiceRegistration.ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        _recipeService = serviceProvider.GetService<IRecipeService>();
        _emailClient = serviceProvider.GetService<IEmailClient>();
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(ScheduledEvent input, ILambdaContext context)
    {
        Console.WriteLine($"--> Scheduled Task about to run.");
        var recommendedRecipes = await _recipeService.GetRandomRecipesByBook("Green");

        try
        {
            Console.WriteLine($"--> Sending email using SES.");
            await _emailClient.SendEmail(recommendedRecipes);
        }
        catch (Exception e)
        {
            Console.WriteLine($"---> Exception thrown: {e.Message}.");
        }
        
        return "End Test";
    }
}