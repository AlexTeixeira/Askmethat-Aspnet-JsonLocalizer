using System;
using System.Globalization;
using System.Text;
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
    public class AbsolutePathJsonFileTest
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
                                    opt.IsAbsolutePath = true;
                                    opt.ResourcesPath = $"{AppContext.BaseDirectory}/path";
                                });
                                this.services = serv;
                            })
                            .UseStartup<Startup>();

            server = new TestServer(builder);

        }

        [TestMethod]
        public void TestReadName1_AbsolutePath_StringLocation()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create("",$"{AppContext.BaseDirectory}/path");

            var result = localizer.GetString("Name1");

            Assert.AreEqual("Mon Nom 1", result);
        }

        [TestMethod]
        public void TestReadName1_AbsolutePath_TypeLocalizer()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("Name1");

            Assert.AreEqual("Mon Nom 1", result);
        }
        
    }
}