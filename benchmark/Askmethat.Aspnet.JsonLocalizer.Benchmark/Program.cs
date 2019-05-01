using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Benchmark.Helpers;
using Microsoft.Extensions.Localization;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Options;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using Askmethat.Aspnet.JsonLocalizer.Benchmark.Resources;
using System.Reflection;

namespace Askmethat.Aspnet.JsonLocalizer.Benchmark
{
    [MinColumn, MaxColumn, MemoryDiagnoser, MarkdownExporter]
    public class BenchmarkJSONLocalizer
    {
        IHostingEnvironment env = new HostingEnvironment();
        private const int N = 10000;
        IStringLocalizer _jsonLocalizer;

        public BenchmarkJSONLocalizer()
        {
            var serviceProvider = new ServiceCollection()
                                     .AddLocalization(opts => { opts.ResourcesPath = "Resources"; })
                                     .BuildServiceProvider();

            _jsonLocalizer = serviceProvider.GetService<IStringLocalizer>();
        }

        [Benchmark]
        public string JsonLocalizer() {
            System.Diagnostics.Debug.WriteLine(_jsonLocalizer.GetString("BaseName1"));
            return _jsonLocalizer.GetString("BaseName1");
        }


        [Benchmark(Baseline = true)]
        public string Localizer() => SharedResources.BaseName1;
    }

    class Program
    {
        static void Main(string[] args)
        {
            //IHostingEnvironment env = new HostingEnvironment();
            //var t = new JsonStringLocalizerFactory(env);
            //var x = t.Create("", "");
            //x.GetString("BaseName1");
            //x.GetString("BaseName2");
            var summary = BenchmarkRunner.Run<BenchmarkJSONLocalizer>();


        }
    }
}
