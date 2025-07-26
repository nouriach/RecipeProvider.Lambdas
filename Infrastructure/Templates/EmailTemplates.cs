using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using RecipeProvider.Lambdas.Config;

namespace RecipeProvider.Lambdas.Infrastructure.Templates;

public static class EmailTemplates
{
    public const string RecipeRecommendationTemplate = "RecipeRecommendationTemplate";

    public static async Task InitializeTemplates(IAmazonSimpleEmailService emailService)
    {
      if (await DoesEmailTemplateExist(emailService)) 
        return;

      var recipeRecTemplate = new CreateTemplateRequest
      {
        Template = new Template
        {
          TemplateName = RecipeRecommendationTemplate,
          SubjectPart = $"This week's dinners: {DateTime.Now:dd/MM/yyyy}",
          HtmlPart =
            """
            <!DOCTYPE html>
            <html>
            <head>
              <meta charset="UTF-8" />
              <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
              <title>Weekly Recipes</title>
              <style>
                body {
                  font-family: Arial, sans-serif;
                  background-color: #f8f9fa;
                  margin: 0;
                  padding: 0;
                  color: #333;
                }
                .container {
                  max-width: 600px;
                  margin: auto;
                  background: #ffffff;
                  padding: 20px;
                  border-radius: 8px;
                }
                h1 {
                  color: #4CAF50;
                }
                .recipe {
                  border-bottom: 1px solid #ddd;
                  padding-bottom: 15px;
                  margin-bottom: 15px;
                }
                .ingredients {
                  padding-left: 20px;
                }
                .footer {
                  font-size: 12px;
                  color: #888;
                  text-align: center;
                  margin-top: 20px;
                }
              </style>
            </head>
            <body>
              <div class="container">
                <h1>Hello {{name}},</h1>
                <p>Here are this week's recipes, handpicked for you!</p>
            
                {{recipes_html}}
            
                <div class="footer">
                  You're receiving this email because you subscribed to Weekly Recipes.
                </div>
              </div>
            </body>
            </html>
            """
        }
      };

      try
      {
        // delete the template, just in case it exists already
        await emailService.DeleteTemplateAsync(
          new DeleteTemplateRequest { TemplateName = RecipeRecommendationTemplate });
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw;
      }

      await emailService.CreateTemplateAsync(recipeRecTemplate);
    }

    private static async Task<bool> DoesEmailTemplateExist(IAmazonSimpleEmailService emailService)
    {
      try
      {
        var emailTemplates = await emailService.GetTemplateAsync(new GetTemplateRequest()
        {
          TemplateName = RecipeRecommendationTemplate
        });

        Console.WriteLine(
          $"---> Does Email Template '{RecipeRecommendationTemplate}' exist? {emailTemplates.Template is not null}");
        return emailTemplates.Template is not null;
      }
      catch (TemplateDoesNotExistException exception)
      {
        Console.WriteLine($"Failed to retrieve template: {exception.Message}");
        return false;
      }
    }
}