using System.Net;
using Amazon;
using Amazon.Lambda.CloudWatchEvents.ScheduledEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleEmail;
using FluentEmail.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using RecipeProvider.Lambdas.Config;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace RecipeProvider.Lambdas;

public class Function
{
    private AmazonSimpleEmailServiceClient AmazonSimpleEmailServiceClient;

    public Function()
    {
        var serviceCollection = new ServiceCollection();
        Console.WriteLine("---> About to call ConfigureServices");
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        AmazonSimpleEmailServiceClient = new AmazonSimpleEmailServiceClient();
    }
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(ScheduledEvent input, ILambdaContext context)
    {
        // At the moment this sends an email when testing in AWS
        // Next step is to sync up DynamoDB

        Console.WriteLine($"--> Scheduled Task about to run.");

        var emailContent = new EmailData()
        {
            ToAddresses = new List<Address>
            {
                new () { Name = "Nathan", EmailAddress = "nouriach17@gmail.com" }
            },
            FromAddress = new Address { EmailAddress = "nouriach17@gmail.com" },
            Subject = "Test from Recipe App Lambda",
            Body = $"Test: {DateTime.UtcNow.ToShortTimeString()}",
        };

        try
        {
            Console.WriteLine($"--> Sending email using SES.");

            var sendRequest = new Amazon.SimpleEmail.Model.SendEmailRequest
            {
                Source = emailContent.FromAddress.EmailAddress,
                Destination = new Amazon.SimpleEmail.Model.Destination
                {
                    ToAddresses = new List<string> { emailContent.ToAddresses[0].EmailAddress }
                },
                Message = new Amazon.SimpleEmail.Model.Message
                {
                    Subject = new Amazon.SimpleEmail.Model.Content(emailContent.Subject),
                    Body = new Amazon.SimpleEmail.Model.Body
                    {
                        Text = new Amazon.SimpleEmail.Model.Content(emailContent.Body)
                    }
                }
            };

            var response = await AmazonSimpleEmailServiceClient.SendEmailAsync(sendRequest);
            Console.WriteLine(response.HttpStatusCode == HttpStatusCode.OK ? "---> Email sent successfully." : "---> Email failed to send.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"---> Exception thrown: {e.Message}.");
        }
        
        return "End Test";
    }
    
    private void ConfigureServices(IServiceCollection serviceCollection)
    {
        Console.WriteLine("---> In ConfigureServices.");
        var lambdaConfig = new LambdaConfiguration();
        serviceCollection.AddSingleton<ILambdaConfiguration>(lambdaConfig);

        Console.WriteLine("---> Registering DynamoDB.");
        var dynamoDbConfig = lambdaConfig.Configuration.GetSection(DynamoDbSettings.SectionName);
        
        serviceCollection.Configure<DynamoDbSettings>(dynamoDbConfig);
        // This isn't working
        // serviceCollection.AddSingleton<IAmazonDynamoDB>(sp =>
        // {
        //     return new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        // });
        
        Console.WriteLine("---> Registering SES.");
        var sesConfig = new AmazonSimpleEmailServiceConfig
        {
            RegionEndpoint = RegionEndpoint.USEast1 // Set your desired region
        };

        serviceCollection.AddSingleton<IAmazonSimpleEmailService>(new AmazonSimpleEmailServiceClient(sesConfig));
    }
}