using Amazon.DynamoDBv2;
using Amazon.SimpleEmail;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeProvider.Lambdas.Application.Abstractions;
using RecipeProvider.Lambdas.Application.Services;
using RecipeProvider.Lambdas.Config;
using RecipeProvider.Lambdas.Infrastructure;
using RecipeProvider.Lambdas.Persistence.Repositories;

namespace RecipeProvider.Lambdas.Extensions;

public static class ServiceRegistration
{
    public static void ConfigureServices(IServiceCollection serviceCollection)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddParameterStoreConfiguration(options =>
            {
                options.SsmClientFactory = () => new AmazonSimpleSystemsManagementClient();
            })
            .Build();

        serviceCollection.Configure<DynamoDbSettings>(config.GetSection(DynamoDbSettings.SectionName));

        RegisterServices(serviceCollection, config);
        RegisterAwsServices(serviceCollection, config);
    }

    private static void RegisterAwsServices(IServiceCollection serviceCollection, IConfigurationRoot config)
    {
        var awsOptions = config.GetAWSOptions();
        serviceCollection.AddDefaultAWSOptions(awsOptions);
        serviceCollection.AddAWSService<IAmazonDynamoDB>();
        serviceCollection.AddAWSService<IAmazonSimpleEmailService>();
    }
    
    private static void RegisterServices(IServiceCollection serviceCollection, IConfigurationRoot config)
    {
        serviceCollection.AddSingleton<IConfiguration>(config);

        serviceCollection.AddScoped<IEmailClient, SimpleEmailClient>();
        serviceCollection.AddScoped<IRecipeService, RecipeService>();
        serviceCollection.AddScoped<IRecipeRepository, RecipeRepository>();
    }
}