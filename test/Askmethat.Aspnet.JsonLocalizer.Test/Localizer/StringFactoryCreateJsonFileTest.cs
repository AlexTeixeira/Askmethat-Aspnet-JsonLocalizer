using System.Globalization;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.TestSample;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class StringFactoryCreateJsonFileTest
    {
        
        IServiceCollection services;
        TestServer server;

        [TestInitialize]
        public void Init()
        {
            var builder = new WebHostBuilder()
                            .ConfigureServices(serv =>
                            {
                                serv.AddJsonLocalization(opt =>
                                {
                                    opt.ResourcesPath = $"/factory";
                                    opt.UseBaseName = true;
                                });
                                this.services = serv;
                            })
                            .UseStartup<Startup>();

            server = new TestServer(builder);

        }

        [TestMethod]
        public void TestReadName1_StringLocation()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create("",$"factory");

            var result = localizer.GetString("Name1");

            Assert.AreEqual("Mon Nom 1", result);
        }

        [TestMethod]
        public void TestReadName1_BaseName_StringLocation()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create("base", $"factory");

            var result = localizer.GetString("Name3");

            Assert.AreEqual("Mon Nom 3", result);

            result = localizer.GetString("Name4");

            Assert.AreEqual("Mon Nom 4", result);

            result = localizer.GetString("Name1");

            Assert.IsTrue(result.ResourceNotFound);

        }
        
    }
}