using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Askmethat.Aspnet.JsonLocalizer.Benchmark.Helpers
{
    public class HostingEnvironment : IHostingEnvironment
    {
        public string EnvironmentName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string ApplicationName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string WebRootPath { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IFileProvider WebRootFileProvider { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string ContentRootPath { get => Directory.GetCurrentDirectory(); set => throw new System.NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}