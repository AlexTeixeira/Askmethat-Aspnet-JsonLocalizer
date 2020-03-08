using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Benchmark.Helpers;
using Microsoft.Extensions.Localization;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using System.Globalization;
using Microsoft.Extensions.Options;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Askmethat.Aspnet.JsonLocalizer.Benchmark.Resources;
using Microsoft.Extensions.FileProviders;

namespace Askmethat.Aspnet.JsonLocalizer.Benchmark
{
    public class HostingEnvironmentStub : IWebHostEnvironment
    {
        public HostingEnvironmentStub()
        {
        }

        public string EnvironmentName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string WebRootPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IFileProvider WebRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ContentRootPath { get => AppContext.BaseDirectory; set => throw new NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    [MinColumn, MaxColumn, MemoryDiagnoser, MarkdownExporter]
    public class BenchmarkJSONLocalizer
    {
        private readonly IWebHostEnvironment env = new HostingEnvironment();
        private readonly IMemoryCache _cach = new MemoryCache(Options.Create<MemoryCacheOptions>(new MemoryCacheOptions() {}));
        private readonly IMemoryCache _cach2 = new MemoryCache(Options.Create<MemoryCacheOptions>(new MemoryCacheOptions() { }));

        private readonly IStringLocalizer _jsonLocalizer;

        public BenchmarkJSONLocalizer()
        {
            _jsonLocalizer = new JsonStringLocalizer(Options.Create<JsonLocalizationOptions>(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("fr-FR"),
                ResourcesPath = "Resources",
                Caching = _cach2
            }), new HostingEnvironmentStub());

            // Simulate the first web request.
            // Ignore *.json loading in test.
            CultureInfo.CurrentUICulture = new CultureInfo("pt-PT");
            _ = _jsonLocalizer.GetString("BaseName1").Value;
            CultureInfo.CurrentUICulture = new CultureInfo("fr-FR");
            _ = _jsonLocalizer.GetString("BaseName1").Value;
        }

        [Benchmark(Baseline = true)]
        public string Localizer()
        {
            return SharedResources.BaseName1;
        }

        [Benchmark]
        public string JsonLocalizer()
        {
            return _jsonLocalizer.GetString("BaseName1").Value;
        }

        [Benchmark]
        public string JsonLocalizerWithCreation()
        {
            var localizer = new JsonStringLocalizer(Options.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("fr-FR"),
                ResourcesPath = "Resources",
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                    new CultureInfo("fr-FR"),
                    new CultureInfo("en-US"),
                }
            }), new HostingEnvironmentStub());

            return localizer.GetString("BaseName1");
        }

        [Benchmark]
        public string JsonLocalizerWithCreationAndExternalMemoryCache()
        {
            JsonStringLocalizer localizer = new JsonStringLocalizer(Options.Create(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("fr-FR"),
                ResourcesPath = "Resources",
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                    new CultureInfo("fr-FR"),
                    new CultureInfo("en-US"),
                },
                Caching = _cach
            }), new HostingEnvironmentStub());

            return localizer.GetString("BaseName1");
        }

        [Benchmark]
        public string JsonLocalizerDefaultCultureValue()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("pt-PT");
            return _jsonLocalizer.GetString("BaseName1").Value;
        }

        [Benchmark]
        public string LocalizerDefaultCultureValue()
        {
            CultureInfo.CurrentUICulture = new CultureInfo("pt-PT");
            return SharedResources.BaseName1;
        }

    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            _ = BenchmarkRunner.Run<BenchmarkJSONLocalizer>();
        }
    }
}
