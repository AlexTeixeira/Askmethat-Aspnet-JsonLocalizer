using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class JsonI18nLocalizer
    {
        private JsonLocalizationOptions _jsonLocalizationOptions = new JsonLocalizationOptions()
        {
            DefaultCulture = new CultureInfo("fr-FR"),
            SupportedCultureInfos = new HashSet<CultureInfo>()
            {
                new CultureInfo("fr-FR"),
                new CultureInfo("en-US")
            },
            ResourcesPath = $"{AppContext.BaseDirectory}/i18n",
            LocalizationMode = LocalizationMode.I18n
        };
        [TestInitialize]
        public void Init()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "UseBaseName can't be activated with I18n LocalisationMode")]
        public void I18n_OptionValidation()
        {
            // Arrange           
            JsonLocalizer.Localizer.JsonStringLocalizer localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("fr-FR"),
                ResourcesPath = $"{AppContext.BaseDirectory}/i18n",
                LocalizationMode = LocalizationMode.I18n,
                UseBaseName = true
            });
        }
        
        [TestMethod]
        public void I18n_GetNameTranslation_ShouldBeFrenchTranslation()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            // Arrange           
            JsonLocalizer.Localizer.JsonStringLocalizer localizer = JsonStringLocalizerHelperFactory.Create(_jsonLocalizationOptions);

            var result = localizer.GetString("Name");

            Assert.AreEqual("Nom",result.Value);
        }
        
        [TestMethod]
        public void I18n_GetColorTranslation_ShouldBeUSTranslation()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            // Arrange           
            JsonLocalizer.Localizer.JsonStringLocalizer localizer = JsonStringLocalizerHelperFactory.Create(_jsonLocalizationOptions);

            var result = localizer.GetString("Color");

            Assert.AreEqual("Color",result.Value);
        }
        
        [TestMethod]
        public void I18n_GetNameTranslation_ShouldBeFrenchThenUSTranslation()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");

            // Arrange           
            JsonLocalizer.Localizer.JsonStringLocalizer localizer = JsonStringLocalizerHelperFactory.Create(_jsonLocalizationOptions);

            var frResult = localizer.GetString("Name");

            Assert.AreEqual("Nom",frResult.Value);
            
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            var usResult = localizer.GetString("Name");

            Assert.AreEqual("Nom",frResult.Value);
        }
        
        [TestMethod]
        public void I18n_Should_Read_Base_NotFound()
        {
            // Arrange
            JsonLocalizer.Localizer.JsonStringLocalizer localizer = JsonStringLocalizerHelperFactory.Create(_jsonLocalizationOptions);
            LocalizedString result = localizer.GetString("Nop");

            Assert.AreEqual("Nop", result);

        }
    }
}