using System;
using System.Collections.Generic;
using System.Globalization;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.TestSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJsonStringLocalizer _localizer;
        public HomeController(IJsonStringLocalizer<HomeController> localizer)
        {
            _localizer = localizer;
            _localizer.ClearMemCache(new List<CultureInfo>()
            {
                new CultureInfo("en-US")
            });
        }
        public IActionResult Index()
        {
            ViewData["Hello"] = _localizer["Hello"] + " " + _localizer["Controller"];
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

    }
}