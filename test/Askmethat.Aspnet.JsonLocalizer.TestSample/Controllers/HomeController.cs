using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.TestSample.Controllers
{
    public class HomeController : Controller
    {
        IStringLocalizer _localizer;
        public HomeController(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }
        public IActionResult Index()
        {
            ViewData["Hello"] = _localizer["Hello"];
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