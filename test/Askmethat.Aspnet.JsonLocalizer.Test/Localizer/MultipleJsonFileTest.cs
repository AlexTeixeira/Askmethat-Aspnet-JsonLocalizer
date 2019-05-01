using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
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
        JsonStringLocalizer localizer = null;
        public void InitLocalizer(CultureInfo cultureInfo)
        {
            CultureInfo.CurrentUICulture = cultureInfo;
            localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("en-US"),
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr-FR"),
                     new CultureInfo("pt-PT"),
                     new CultureInfo("it-IT"),
                },
                ResourcesPath = "multiple",
            });
        }

        [TestMethod]
        public void Should_Read_Name1()
        {
            // Arrange
            InitLocalizer(new CultureInfo("fr-FR"));

            var result = localizer.GetString("Name1");

            Assert.AreEqual("Mon Nom 1", result);
        }


        [TestMethod]
        public void Should_Read_Name1_PT()
        {
            // Arrange
            InitLocalizer(new CultureInfo("pt-PT"));

            var result = localizer.GetString("Name1");

            Assert.AreEqual("o meu nome 1", result);
        }

        [TestMethod]
        public void Should_Read_Name2_IT()
        {
            // Arrange
            InitLocalizer(new CultureInfo("it-IT"));

            var result = localizer.GetString("Name2");

            Assert.AreEqual("il mio nome 2", result);
        }

    }
}
