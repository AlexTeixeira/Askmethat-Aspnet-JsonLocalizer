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
using Microsoft.Extensions.FileProviders;
using System.Threading;

namespace Askmethat.Aspnet.JsonLocalizer.Benchmark
{
    public class HostingEnvironmentStub : IHostingEnvironment
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
        IHostingEnvironment env = new HostingEnvironment();
        IMemoryCache _cach = new MemoryCache(Options.Create<MemoryCacheOptions>(new MemoryCacheOptions() {}));
        IMemoryCache _cach2 = new MemoryCache(Options.Create<MemoryCacheOptions>(new MemoryCacheOptions() { }));

        IStringLocalizer _jsonLocalizer;

        public BenchmarkJSONLocalizer()
        {
            _jsonLocalizer = new JsonStringLocalizer(Options.Create<JsonLocalizationOptions>(new JsonLocalizationOptions()
            {
                DefaultCulture = new CultureInfo("fr-FR"),
                ResourcesPath = "Resources",
                Caching = _cach2
            }), new HostingEnvironmentStub());
        }

        [Benchmark(Baseline = true)]
        public string Localizer() => SharedResources.BaseName1;

        [Benchmark]
        public string JsonLocalizer() => _jsonLocalizer.GetString("BaseName1").Value;

        [Benchmark]
        public string JsonLocalizerWithCreation()
        {
            var localizer = new JsonStringLocalizer(Options.Create<JsonLocalizationOptions>(new JsonLocalizationOptions()
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
            var localizer = new JsonStringLocalizer(Options.Create<JsonLocalizationOptions>(new JsonLocalizationOptions()
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
            BenchmarkRunner.Run<BenchmarkJSONLocalizer>();
        }
    }
}
