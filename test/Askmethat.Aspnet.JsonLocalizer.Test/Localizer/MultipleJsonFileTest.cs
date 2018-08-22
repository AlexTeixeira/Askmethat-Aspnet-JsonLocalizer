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

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class MultipleJsonFileTest
    {
        IServiceCollection services;
        TestServer server;

        [TestInitialize]
        public void Init()
        {
            var builder = new WebHostBuilder()
                            .ConfigureServices(serv =>
                            {
                                serv.AddJsonLocalization(options => options.ResourcesPath = "multiple");
                                this.services = serv;
                            })
                            .UseStartup<Startup>();

            server = new TestServer(builder);

        }

        [TestMethod]
        public void Should_Read_Name1()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));


            var result = localizer.GetString("Name1");

            Assert.AreEqual("Mon Nom 1", result);
        }


        [TestMethod]
        public void Should_Read_Name1_PT()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("pt-PT");

            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));


            var result = localizer.GetString("Name1");

            Assert.AreEqual("o meu nome 1", result);
        }

        [TestMethod]
        public void Should_Read_Name2_IT()
        {
            // Arrange
            CultureInfo.CurrentUICulture = new CultureInfo("it-IT");

            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));


            var result = localizer.GetString("Name2");

            Assert.AreEqual("il mio nome 2", result);
        }

    }
}
