using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Configuration;

namespace RecipeProvider.Lambdas.Config;

public class ParameterStoreConfigurationSource : IConfigurationSource
{
    public Func<IAmazonSimpleSystemsManagement> SsmClientFactory { get; set; }
    public string FromEmail {get; set; } = "/email/send_from";
    public string ToEmail {get; set; } = "/email/send_to";

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new ParameterStoreConfigurationProvider(this, SsmClientFactory);
    }
}