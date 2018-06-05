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
    public class EncodedJsonFileTest
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
                                    opt.ResourcesPath = "encoding";
                                    opt.FileEncoding = Encoding.GetEncoding("ISO-8859-1");
                                });
                                this.services = serv;
                            })
                            .UseStartup<Startup>();

            server = new TestServer(builder);

        }

        [TestMethod]
        public void TestReadName1_ISOEncoding()
        {
            CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("Name1");

            Assert.AreEqual("Mon Nom 1", result);
        }

        [TestMethod]
        public void TestReadName1_ISOEncoding_SpecialChar()
        {
            CultureInfo.CurrentCulture = new CultureInfo("pt-PT");
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("Name1");

            Assert.AreEqual("Eu so joão", result);
        }

    }
}
