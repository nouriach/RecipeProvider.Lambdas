using System.Net;
using System.Text;
using System.Text.Json;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RecipeProvider.Lambdas.Application.Abstractions;
using RecipeProvider.Lambdas.Application.Models;
using RecipeProvider.Lambdas.Config;
using RecipeProvider.Lambdas.Infrastructure.Templates;

namespace RecipeProvider.Lambdas.Infrastructure;

public class SimpleEmailClient(IAmazonSimpleEmailService simpleEmailClient, IConfiguration config) : IEmailClient
{
    public async Task SendEmail(List<Recipe> emailContent)
    {
        await EmailTemplates.InitializeTemplates(simpleEmailClient);

        try
        {
            Console.WriteLine($"--> Sending email using SES.");

            var toEmail = config["/email/send_to"];
            var fromEmail = config["/email/send_from"];
            if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(fromEmail))
                throw new Exception("Failed to retrieve email from SSM");

            Console.WriteLine($"Retrieving To and From emails from config: {toEmail}, {fromEmail}");

            var sendRequest = new SendTemplatedEmailRequest()
            {
                Source = fromEmail, 
                Template = EmailTemplates.RecipeRecommendationTemplate,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { toEmail }
                },
                TemplateData = JsonSerializer.Serialize(new {name = "Nathan", recipes_html = BuildHtmlBody(emailContent)})
            };

            var response = await simpleEmailClient.SendTemplatedEmailAsync(sendRequest);
            Console.WriteLine(response.HttpStatusCode == HttpStatusCode.OK ? "---> Email sent successfully." : "---> Email failed to send.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"---> Exception thrown: {e.Message}.");
        }
    }

    private string BuildHtmlBody(List<Recipe> emailContent)
    {
        var recipesHtml = new StringBuilder();
        foreach (var recipe in emailContent)
        {
            recipesHtml.Append($@"
                <div class='recipe'>
                    <h2>{recipe.Title}</h2>
                    <p><strong>Prep Time:</strong> {recipe.Summary.PrepTime} |
                       <strong>Cook Time:</strong> {recipe.Summary.CookTime} |
                       <strong>Serves:</strong> {recipe.Summary.Serves}</p>
                    <p><strong>Ingredients:</strong></p>
                    <ul>
                        {string.Join("", recipe.Ingredients.Select(i => $"<li>{i}</li>"))}
                    </ul>
                </div>
            ");
        }

        return recipesHtml.ToString();
    }
}