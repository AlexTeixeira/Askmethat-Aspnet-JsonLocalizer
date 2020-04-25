using Askmethat.Aspnet.JsonLocalizer.Test.Helpers;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Text;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;

namespace Askmethat.Aspnet.JsonLocalizer.Test.Localizer
{
    [TestClass]
    public class EncodedJsonFileTest
    {

        [TestMethod]
        public void TestReadName1_ISOEncoding()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            JsonLocalizer.Localizer.JsonStringLocalizer localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("en-US"),
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr-FR")
                },
                ResourcesPath = "encoding",
                FileEncoding = Encoding.GetEncoding("ISO-8859-1")
            });

            LocalizedString result = localizer.GetString("Name1");

            Assert.AreEqual("Mon Nom 1", result);
        }

        [TestMethod]
        public void TestReadName1_ISOEncoding_SpecialChar()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("pt-PT");
            JsonLocalizer.Localizer.JsonStringLocalizer localizer = JsonStringLocalizerHelperFactory.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("en-US"),
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                     new CultureInfo("fr-FR"),
                     new CultureInfo("pt-PT")
                },
                ResourcesPath = "encoding",
                FileEncoding = Encoding.GetEncoding("ISO-8859-1")
            });

            LocalizedString result = localizer.GetString("Name1");

            Assert.AreEqual("Eu so joão", result);
        }

    }
}
