using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;

namespace Askmethat.Aspnet.JsonLocalizer.Extensions
{
    /// <summary>
    /// Extension method for setting up services in a <see cref="IServiceCollection" />.
    /// </summary>
    public static class JsonLocalizerServiceExtension
    {
        /// <summary>
        /// Add JsonLocalization to asp.net service collection
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddJsonLocalization(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            _ = services.AddOptions();

            AddJsonLocalizationServices(services);

            return services;
        }

        /// <summary>
        /// Add JsonLocalization to asp.net service collection and localization options
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddJsonLocalization(
            this IServiceCollection services,
            Action<JsonLocalizationOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                Console.Error.WriteLine("Setup Action seems to be null, The localization options will not be override. For any helps create an issue at ");
                AddJsonLocalizationServices(services);
            }

            AddJsonLocalizationServices(services, setupAction);

            return services;
        }


        internal static void AddJsonLocalizationServices(IServiceCollection services)
        {
            _ = services.AddMemoryCache();
            _ = services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            _ = services.AddScoped<IJsonStringLocalizer, JsonStringLocalizer>();
            _ = services.AddScoped(typeof(IJsonStringLocalizer<>), typeof(JsonStringLocalizerOfT<>));
        }

        /// <summary>
        /// If user want to override default localization options
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to</param>
        /// <param name="setupAction">The <see cref="LocalizationOptions"/> option to change default localization mecanism</param>
        internal static void AddJsonLocalizationServices(
            IServiceCollection services,
            Action<JsonLocalizationOptions> setupAction)
        {
            AddJsonLocalizationServices(services);
            _ = services.Configure(setupAction);
        }
    }
}
