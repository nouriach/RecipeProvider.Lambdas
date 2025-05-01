using Microsoft.Extensions.Configuration;

namespace RecipeProvider.Lambdas.Config
{
    public class LambdaConfiguration : ILambdaConfiguration
    {
        public IConfiguration Configuration { get; }

        public LambdaConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory) // Use AppContext to handle Lambda's directory structure
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load JSON in local development
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true) // Support environment-based configs
                .AddEnvironmentVariables(); // Load AWS Lambda environment variables

            Configuration = builder.Build();
        }
    }
}
