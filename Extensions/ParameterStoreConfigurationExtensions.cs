using Microsoft.Extensions.Configuration;
using RecipeProvider.Lambdas.Config;

namespace RecipeProvider.Lambdas.Extensions;

public static class ParameterStoreConfigurationExtensions
{
    public static IConfigurationBuilder AddParameterStoreConfiguration(
        this IConfigurationBuilder builder, Action<ParameterStoreConfigurationSource> configure)
    {
        var source = new ParameterStoreConfigurationSource();
        configure(source);
        builder.Add(source);
        return builder;
    }
}