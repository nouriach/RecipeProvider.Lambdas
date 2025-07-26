using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;

namespace RecipeProvider.Lambdas.Config;

public class ParameterStoreConfigurationProvider(ParameterStoreConfigurationSource source, Func<IAmazonSimpleSystemsManagement> ssmClientFactory) : ConfigurationProvider
{
    // interact with SSMS
    public override void Load()
    {
        try
        {
            Console.WriteLine("---> Inside Load()");
            var configData = GetSettingsAsync();
            Data = configData;
            OnReload();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading API config: {ex.Message}");
        }
    }

    private Dictionary<string, string> GetSettingsAsync()
    {
        var ssm = ssmClientFactory.Invoke();
        var emails = new Dictionary<string, string>();
        GetParameterResponse fromEmail;
        Console.WriteLine("Retrieving fromEmail");
        try
        {
            fromEmail = ssm.GetParameterAsync(new GetParameterRequest()
            {
                Name = source.FromEmail,
                WithDecryption = true
            }).ConfigureAwait(false).GetAwaiter().GetResult();;
            Console.WriteLine($"Retrieved fromEmail: {fromEmail}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"---> {e.Message}");
            throw;
        }

        Console.WriteLine("Retrieving toEmail");
        var toEmail = ssm.GetParameterAsync(new GetParameterRequest()
        {
            Name = source.ToEmail,
            WithDecryption = true
        }).ConfigureAwait(false).GetAwaiter().GetResult();
        Console.WriteLine($"Retrieved fromEmail: {toEmail}");

        emails.Add(source.FromEmail, fromEmail.Parameter.Value);
        emails.Add(source.ToEmail, toEmail.Parameter.Value);

        return emails;
    }
}