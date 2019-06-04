using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.TestSample.Pages
{
    public class HelloRazorModel : PageModel
    {
        private readonly IStringLocalizer<HelloRazorModel> localizer;

        public HelloRazorModel(IStringLocalizer<HelloRazorModel> localizer)
        {
            this.localizer = localizer;
            Hello = this.localizer["Hello"];
        }

        public string Hello { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "InputField")]
            public string InputField { get; set; }
        }

        public void OnGet()
        {

        }
    }
}