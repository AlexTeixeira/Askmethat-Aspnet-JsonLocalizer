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
        MemoryCache mem = new MemoryCache(new MemoryCacheOptions
        {
        });

        JsonStringLocalizerFactory _jsonFactory;
        IStringLocalizer _jsonLocalizer;

        public BenchmarkJSONLocalizer()
        {
            var serviceProvider = new ServiceCollection()
                                      .AddLocalization(opts => { opts.ResourcesPath = "Resources"; })
                                     .BuildServiceProvider();

            _jsonFactory = new JsonStringLocalizerFactory(env, mem);
            _jsonLocalizer = _jsonFactory.Create("", "");
        }

        [Benchmark]
        public string JsonLocalizer() => _jsonLocalizer.GetString("BaseName1");


        [Benchmark(Baseline = true)]
        public string Localizer() => SharedResources.BaseName1;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchmarkJSONLocalizer>();
        }
    }
}
