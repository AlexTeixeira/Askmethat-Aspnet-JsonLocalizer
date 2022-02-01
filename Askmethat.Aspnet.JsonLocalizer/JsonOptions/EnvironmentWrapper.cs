using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Askmethat.Aspnet.JsonLocalizer.JsonOptions
{
    public class EnvironmentWrapper
    {
#if NETCORE
        private IWebHostEnvironment serverEnvironment;

        public EnvironmentWrapper(IWebHostEnvironment hostingEnvironmentStub)
        {
            this.serverEnvironment = hostingEnvironmentStub;
        }

#endif
        public string ContentRootPath { get; internal set; }
        public bool IsWasm { get; } = false;


    }
}
