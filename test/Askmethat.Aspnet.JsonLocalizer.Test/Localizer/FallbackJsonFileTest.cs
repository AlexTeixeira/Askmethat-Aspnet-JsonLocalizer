using System;
using System.Collections;

using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.TestSample;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Linq;

using LocalizedString = Microsoft.Extensions.Localization.LocalizedString;


namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class FallbackJsonFileTest
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
                                    opt.ResourcesPath = "fallback";
                                    opt.DefaultCulture = null;
                                });
                                this.services = serv;
                            })
                            .UseStartup<Startup>();

            server = new TestServer(builder);

        }

        [TestMethod]
        public void Should_Read_Color_NoFallback()
        {
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();

            SetCurrentCulture("en-AU");
            var localizer = factory.Create(typeof(IStringLocalizer));
            var result = localizer.GetString("Color");
            Assert.AreEqual("Colour (specific)", result);

            SetCurrentCulture("fr");
            localizer = factory.Create(typeof(IStringLocalizer));
            result = localizer.GetString("Color");
            Assert.AreEqual("Couleur (neutral)", result);

            SetCurrentCulture(CultureInfo.InvariantCulture);
            localizer = factory.Create(typeof(IStringLocalizer));
            result = localizer.GetString("Color");
            Assert.AreEqual("Color (invariant)", result);
        }

        [TestMethod]
        public void Should_Read_Color_FallbackToParent()
        {
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();

            SetCurrentCulture("fr-FR");
            var localizer = factory.Create(typeof(IStringLocalizer));
            var result = localizer.GetString("Color");
            Assert.AreEqual("Couleur (neutral)", result);
            Assert.IsFalse(result.ResourceNotFound);

            SetCurrentCulture("en-NZ");
            localizer = factory.Create(typeof(IStringLocalizer));
            result = localizer.GetString("Color");
            Assert.AreEqual("Color (neutral)", result);
            Assert.IsFalse(result.ResourceNotFound);

            SetCurrentCulture("zh-CN");
            localizer = factory.Create(typeof(IStringLocalizer));
            result = localizer.GetString("Color");
            Assert.AreEqual("Color (invariant)", result);
            Assert.IsFalse(result.ResourceNotFound);

        }

        [TestMethod]
        public void Should_Read_ResourceMissingCulture_FallbackToResourceName()
        {
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();

            SetCurrentCulture("zh-CN");
            var localizer = factory.Create(typeof(IStringLocalizer));
            var result = localizer.GetString("Empty");
            Assert.AreEqual("Empty", result);
            Assert.IsTrue(result.ResourceNotFound);
        }

        [TestMethod]
        public void Should_Read_MissingResource_FallbackToResourceName()
        {
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();

            SetCurrentCulture("en-AU");
            var localizer = factory.Create(typeof(IStringLocalizer));

            var result = localizer.GetString("No resource string");
            Assert.AreEqual("No resource string", result);
            Assert.IsTrue(result.ResourceNotFound);
        }

        [TestMethod]
        public void Should_Read_AllStringsWithParentFallback()
        {
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();

            SetCurrentCulture("en-AU");
            var localizer = factory.Create(typeof(IStringLocalizer));

            var results = localizer.GetAllStrings(includeParentCultures: true).ToArray();
            var expected = new[] {
                new LocalizedString("Color", "Colour (specific)", false),
                new LocalizedString("Empty", "Empty", false)
            };
            CollectionAssert.AreEqual(expected, results, new LocalizedStringComparer());
        }

        [TestMethod]
        public void Should_Read_AllStringsWithoutParentFallback()
        {
            var sp = services.BuildServiceProvider();
            var factory = sp.GetService<IStringLocalizerFactory>();

            SetCurrentCulture("en-AU");
            var localizer = factory.Create(typeof(IStringLocalizer));

            var results = localizer.GetAllStrings(includeParentCultures: false).ToArray();
            var expected = new[] {
                new LocalizedString("Color", "Colour (specific)", false)
            };
            CollectionAssert.AreEqual(expected, results, new LocalizedStringComparer());
        }

        /// <summary>
        /// LocalizedString doesn't implement the IComparer interface required by CollectionAssert.AreEqual(), so providing one here
        /// </summary>
        private class LocalizedStringComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var lsX = (LocalizedString)x;
                var lsY = (LocalizedString)y;
                if (ReferenceEquals(lsX, lsY))
                {
                    return 0;
                }
                if (lsX.Name == lsY.Name && lsX.Value == lsY.Value && lsX.ResourceNotFound == lsY.ResourceNotFound)
                {
                    return 0;
                }
                int result = StringComparer.CurrentCulture.Compare(lsX.Name, lsY.Name);
                if (result != 0)
                {
                    return result;
                }
                result = StringComparer.CurrentCulture.Compare(lsX.Value, lsY.Value);
                if (result != 0)
                {
                    return result;
                }
                return lsX.ResourceNotFound.CompareTo(lsY.ResourceNotFound);
            }
        }

        private void SetCurrentCulture(string cultureName)
            => SetCurrentCulture(new CultureInfo(cultureName));

        private void SetCurrentCulture(CultureInfo cultureInfo)
            => CultureInfo.CurrentUICulture = cultureInfo;
    }
}
