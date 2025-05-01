using Microsoft.Extensions.Configuration;

namespace RecipeProvider.Lambdas.Config;

public interface ILambdaConfiguration
{
    public IConfiguration Configuration { get; }
}