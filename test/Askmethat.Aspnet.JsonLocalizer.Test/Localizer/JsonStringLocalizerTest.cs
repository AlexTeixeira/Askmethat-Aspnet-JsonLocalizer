using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.TestSample;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Linq;


namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class JsonStringLocalizerTest
    {
        IServiceCollection services;
        TestServer server;

        [TestInitialize]
        public void Init()
        {
            var builder = new WebHostBuilder()
                            .ConfigureServices(serv =>
                            {
                                serv.AddJsonLocalization(options => options.DefaultCulture = new CultureInfo("en-US"));
                                this.services = serv;
                            })
                            .UseStartup<Startup>();

            server = new TestServer(builder);

        }


        [TestMethod]
        public void Should_Read_Json_From_Ressource()
        {
            // Arrange
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));
        }


        [TestMethod]
        public void Should_Read_Base_Name1()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));


            var result = localizer.GetString("BaseName1");

            Assert.AreEqual("Mon Nom de Base 1", result);
        }

        [TestMethod]
        public void Should_Read_Base_NotFound()
        {
            // Arrange
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("Nop");

            Assert.AreEqual("Nop", result);

        }

        [TestMethod]
        public void Should_Read_Base_UseDefault()
        {
            // Arrange
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var result = localizer.GetString("NoFrench");

            Assert.AreEqual("No more french", result);
        }

        [TestMethod]
        public void Should_Read_Base_Name1_US()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("BaseName1");

            Assert.AreEqual("My Base Name 1", result);
        }

        [TestMethod]
        public void Should_Read_Default_Name1_US()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("de-DE");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("BaseName1");

            Assert.AreEqual("My Base Name 1", result);
        }

        [TestMethod]
        public void Should_Read_CaseInsensitive_CultureName()
        {
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var result = localizer.GetString("CaseInsensitiveCultureName");
            Assert.AreEqual("French", result);
        }

        [TestMethod]
        public void Should_Read_CaseInsensitive_UseDefault()
        {
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            CultureInfo.CurrentUICulture = new CultureInfo("de-DE");
            var result = localizer.GetString("CaseInsensitiveCultureName");
            Assert.AreEqual("US English", result);
        }

        [TestMethod]
        public void Should_GetAllStrings_ByCaseInsensitiveCultureName()
        {
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var expected = new[] {
                "Mon Nom de Base 1",
                "Mon Nom de Base 2",
                "No more french",
                "French"
            };
            var results = localizer.GetAllStrings().Select(x => x.Value).ToArray();
            CollectionAssert.AreEquivalent(expected, results);
        }
    }
}
