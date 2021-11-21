namespace Scaffold.WebApi.Extensions;

using Microsoft.Extensions.DependencyInjection;

internal static class MvcBuilderExtensions
{
    public static IMvcBuilder AddCustomJsonOptions(this IMvcBuilder builder)
    {
        builder.AddJsonOptions(options =>
        {
        });

        return builder;
    }

    public static IMvcBuilder AddCustomXmlFormatters(this IMvcBuilder builder)
    {
        builder.AddXmlDataContractSerializerFormatters(options =>
        {
        });

        return builder;
    }
}
