using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.JsonOptions
{
    public class EnvironmentWrapper
    {

        private IWebAssemblyHostEnvironment wasmEnvironment;

        public EnvironmentWrapper(IWebAssemblyHostEnvironment hostingEnvironmentStub)
        {
            this.wasmEnvironment = hostingEnvironmentStub;
        }

        public bool IsWasm { get; } = true;

        public string ContentRootPath { get; internal set; }

    }
}
