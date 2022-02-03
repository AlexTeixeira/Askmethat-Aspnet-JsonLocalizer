using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Askmethat.Aspnet.JsonLocalizer.Localizer.Modes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class SystemTextJsonTest
    {
        //[TestMethod]
        //public void I18n_ReadJson_JsonNode()
        //{
        //    var jsonFile = $"{AppContext.BaseDirectory}/i18n/localization.en-US.json";
        //    var doc = JsonNode.Parse(File.ReadAllText(jsonFile, Encoding.UTF8));
           
        //}

        [TestMethod]
        public void I18n_ReadJson_JsonDoc()
        {
            var gen = new LocalizationI18NModeGenerator();
            var jsonFile = $"{AppContext.BaseDirectory}/i18n/localization.fr/localization.fr-FR.json";
            var opts = new JsonLocalizationOptions() { };
            gen.AddValueToLocalization(opts, jsonFile, true);
            var loc = gen.ConstructLocalization(new[] { "" }, System.Globalization.CultureInfo.CurrentCulture, opts);
            Assert.AreEqual(2,loc.Count);
            Assert.AreEqual("Nom", loc["Name"].Value);
        }
    }
}
