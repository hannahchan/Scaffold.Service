namespace Scaffold.WebApi.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public static class MvcCoreBuilderExtension
    {
        public static IMvcCoreBuilder AddCustomJsonFormatters(this IMvcCoreBuilder builder)
        {
            builder.AddJsonFormatters(settings =>
            {
                settings.Converters.Add(new StringEnumConverter { AllowIntegerValues = false });
                settings.NullValueHandling = NullValueHandling.Ignore;
            });

            return builder;
        }

        public static IMvcCoreBuilder AddCustomXmlFormatters(this IMvcCoreBuilder builder)
        {
            builder.AddXmlDataContractSerializerFormatters(settings =>
            {
            });

            return builder;
        }
    }
}
