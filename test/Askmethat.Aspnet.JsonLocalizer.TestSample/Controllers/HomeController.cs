using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.TestSample.Controllers
{
    public class HomeController : Controller
    {
        IStringLocalizer _localizer;
        public HomeController(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(IStringLocalizer));
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}