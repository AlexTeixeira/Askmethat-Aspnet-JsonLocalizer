using System.Collections.Generic;
using System.Globalization;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Microsoft.AspNetCore.Mvc;

namespace Askmethat.Aspnet.JsonLocalizer.TestSample.Controllers
{
    public class ReloadController : Controller
    {
        private readonly IJsonStringLocalizer _localizer;
        public ReloadController(IJsonStringLocalizer<ReloadController> localizer)
        {
            _localizer = localizer;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReloadCultures(string culture, string returnUrl)
        {
            _localizer.ReloadMemCache(new List<CultureInfo>()
            {
                new CultureInfo(culture)
            });

            return LocalRedirect(returnUrl);
        }

    }
} 
