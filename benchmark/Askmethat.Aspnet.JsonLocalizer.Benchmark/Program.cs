using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using Askmethat.Aspnet.JsonLocalizer.Benchmark.Helpers;
using Microsoft.Extensions.Localization;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Askmethat.Aspnet.JsonLocalizer.Benchmark.Resources;
using Askmethat.Aspnet.JsonLocalizer.JsonOptions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

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
            }), new EnvironmentWrapper(new HostingEnvironmentStub()));
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
                ResourcesPath = "i18n",
                SupportedCultureInfos = new System.Collections.Generic.HashSet<CultureInfo>()
                {
                    new CultureInfo("fr-FR"),
                    new CultureInfo("en-US"),
                },
                LocalizationMode = LocalizationMode.I18n
            }), new EnvironmentWrapper(new HostingEnvironmentStub()));

            return localizer.GetString("BaseName1");
        }
        
        [Benchmark]
        public string I18nJsonLocalizerWithCreation()
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
            }), new EnvironmentWrapper(new HostingEnvironmentStub()));

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
            }), new EnvironmentWrapper(new HostingEnvironmentStub()));

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
