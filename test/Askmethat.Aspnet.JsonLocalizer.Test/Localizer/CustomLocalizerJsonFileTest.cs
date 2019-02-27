using System.Globalization;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.TestSample;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class CustomLocalizerJsonFileTest
    {
        IServiceCollection services;
        TestServer server;

        [TestInitialize]
        public void Init()
        {
            var builder = new WebHostBuilder()
                            .ConfigureServices(serv =>
                            {
                                serv.AddJsonLocalization(options =>
                                {
                                    options.DefaultCulture = new CultureInfo("en-US");
                                    options.PluralSeparator = '#';
                                });
                                this.services = serv;
                            })
                            .UseStartup<Startup>();

            server = new TestServer(builder);
        }

        [TestMethod]
        public void Should_Be_Singular_Users_Custom()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("CustomPluralUser", false);

            Assert.AreEqual("Utilisateur", result);
        }

        [TestMethod]
        public void Should_Be_Plural_Users_Custom()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("CustomPluralUser", true);

            Assert.AreEqual("Utilisateurs", result);
        }
    }
}