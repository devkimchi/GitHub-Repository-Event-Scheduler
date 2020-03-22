using System.Net.Http.Formatting;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

[assembly: FunctionsStartup(typeof(EventScheduler.FunctionApp.StartUp))]
namespace EventScheduler.FunctionApp
{
    /// <summary>
    /// This represents the entity for the IoC container.
    /// </summary>
    public class StartUp : FunctionsStartup
    {
        private const string StorageConnectionStringKey = "AzureWebJobsStorage";

        /// <inheritdoc />
        public override void Configure(IFunctionsHostBuilder builder)
        {
            this.ConfigureHttpClient(builder.Services);
            this.ConfigureJsonSerialiser(builder.Services);
        }

        private void ConfigureHttpClient(IServiceCollection services)
        {
            services.AddHttpClient();
        }

        private void ConfigureJsonSerialiser(IServiceCollection services)
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() },
                Converters = { new StringEnumConverter() },
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };

            var formatter = new JsonMediaTypeFormatter()
            {
                SerializerSettings = settings
            };

            services.AddSingleton<JsonSerializerSettings>(settings);
            services.AddSingleton<JsonMediaTypeFormatter>(formatter);
        }
    }
}
