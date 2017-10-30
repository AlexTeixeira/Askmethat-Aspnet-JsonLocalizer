using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.TestSample;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class JsonStringLocalizerCustomResourcesTest
    {
        IServiceCollection services;
        TestServer server;

        [TestInitialize]
        public void Init()
        {
            var builder = new WebHostBuilder()
                            .ConfigureServices(serv =>
                            {
                                serv.AddJsonLocalization(options => options.ResourcesPath = "json");
                                this.services = serv;
                            })
                            .UseStartup<Startup>();

            server = new TestServer(builder);

        }


        [TestMethod]
        public void Should_Read_Custom_Json_From_Ressource()
        {
            // Arrange
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));
        }


        [TestMethod]
        public void Should_Read_Name1()
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("Name1");

            Assert.AreEqual("Mon Nom 1", result);
        }

        [TestMethod]
        public void Should_Read_NotFound()
        {
            // Arrange
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("Nop");

            Assert.AreEqual(String.Empty, result);
        }

        [TestMethod]
        public void Should_Read_Name1_US()
        {
            // Arrange
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("Name1");

            Assert.AreEqual("My Name 1", result);
        }
    }
}
