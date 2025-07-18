using System.Net;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleEmail;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeProvider.Lambdas.Application.Abstractions;
using RecipeProvider.Lambdas.Application.Services;
using RecipeProvider.Lambdas.Config;
using RecipeProvider.Lambdas.Infrastructure;
using RecipeProvider.Lambdas.Persistence.Repositories;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace RecipeProvider.Lambdas;

public class Function
{
    private IRecipeService _recipeService;
    private IEmailService _emailService;
    private IEmailClient _emailClient;
    private ILambdaConfiguration Configuration { get; }

    public Function()
    {
        var serviceCollection = new ServiceCollection();
        var lambdConfig = new LambdaConfiguration();
        Configuration = lambdConfig;
        serviceCollection.AddSingleton<ILambdaConfiguration>(lambdConfig);
        Console.WriteLine("---> About to call ConfigureServices");
        ConfigureServices(serviceCollection, lambdConfig);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        Configuration = serviceProvider.GetService<ILambdaConfiguration>();
        _recipeService = serviceProvider.GetService<IRecipeService>();
        _emailService = serviceProvider.GetService<IEmailService>();
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
        var emailContent = _emailService.BuildEmailFromRecipes(recommendedRecipes);

        try
        {
            Console.WriteLine($"--> Sending email using SES.");
            await _emailClient.SendEmail(emailContent);
        }
        catch (Exception e)
        {
            Console.WriteLine($"---> Exception thrown: {e.Message}.");
        }
        
        return "End Test";
    }
    
    private void ConfigureServices(IServiceCollection serviceCollection, ILambdaConfiguration lambdaConfig)
    {
        Console.WriteLine("---> In ConfigureServices.");

        Console.WriteLine("---> Registering DynamoDB.");
        // var dynamoDbConfig = lambdaConfig.Configuration.GetSection(DynamoDbSettings.SectionName);
        //
        // serviceCollection.Configure<DynamoDbSettings>(dynamoDbConfig);

        var awsOptions = lambdaConfig.Configuration.GetAWSOptions();
        Console.WriteLine($"AWS Region: {awsOptions.Region}");
        Console.WriteLine($"AWS Profile: {awsOptions.Profile}");
        serviceCollection.AddDefaultAWSOptions(awsOptions);
        serviceCollection.AddAWSService<IAmazonDynamoDB>();
        
        Console.WriteLine("---> Registering SES.");
        var sesConfig = new AmazonSimpleEmailServiceConfig
        {
            RegionEndpoint = RegionEndpoint.USEast1 // Set your desired region
        };

        serviceCollection.AddSingleton<IAmazonSimpleEmailService>(new AmazonSimpleEmailServiceClient(sesConfig));
        serviceCollection.AddScoped<IEmailService, EmailService>();
        serviceCollection.AddScoped<IEmailClient, SimpleEmailClient>();
        serviceCollection.AddScoped<IRecipeService, RecipeService>();
        serviceCollection.AddScoped<IRecipeRepository, RecipeRepository>();
    }
}