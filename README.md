# Askmethat-Aspnet-JsonLocalizer
Json Localizer library for .NetStandard and .NetCore Asp.net projects

#### Nuget
[![NuGet](https://img.shields.io/nuget/dt/Askmethat.Aspnet.JsonLocalizer.svg)](https://www.nuget.org/packages/Askmethat.Aspnet.JsonLocalizer)

#### Build

[![Build status](https://ci.appveyor.com/api/projects/status/gt8vg0e2f9gapr2d/branch/master?svg=true)](https://ci.appveyor.com/project/AlexTeixeira/askmethat-aspnet-jsonlocalizer/branch/master)

# Project

This library allows users to use JSON files instead of RESX in an ASP.NET application.
The code tries to be most compliant with Microsoft guidelines.
The library is compatible with NetStandard & NetCore.

# Configuration

An extension method is available for `IServiceCollection`.
You can have a look at the method [here](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/blob/development/Askmethat.Aspnet.JsonLocalizer/Extensions/JsonLocalizerServiceExtension.cs)

## Options 

A set of options is available.
You can define them like this : 

``` cs
services.AddJsonLocalization(options => {
        options.CacheDuration = TimeSpan.FromMinutes(15);
        options.ResourcesPath = "mypath";
        options.FileEncoding = Encoding.GetEncoding("ISO-8859-1");
        options.SupportedCultureInfos = new HashSet<CultureInfo>()
        {
          new CultureInfo("en-US"),
          new CultureInfo("fr-FR")
        };
    });
```

### Current Options

- **SupportedCultureInfos** : _Default value : _List containing only default culture_ and CurrentUICulture. Optionnal array of cultures that you should provide to plugin. _(Like RequestLocalizationOptions)
- **ResourcesPath** : _Default value : `$"{_env.WebRootPath}/Resources/"`_.  Base path of your resources. The plugin will browse the folder and sub-folders and load all present JSON files.
- **CacheDuration** : _Default value : 30 minutes_. Cache all values to memory to avoid loading files for each request,
- **FileEncoding** : _default value : UTF8_. Specify the file encoding.
- **IsAbsolutePath** : *_default value : false*. Look for an absolute path instead of project path.
- **UseBaseName** : *_default value : false*. Use base name location for Views and consors like default Resx localization in **ResourcePathFolder**.
- **Caching** : *_default value: MemoryCache*. Internal caching can be overwritted by using custom class that extends IMemoryCache.
- **PluralSeparator** : *_default value: |*. Seperator used to get singular or pluralized version of localization. More information in *Pluralization*

#Pluralization

In version 2.0.0, Pluralization was introduced.
You are now able to manage a singular (left) and plural (right) version for the same Key. 
*PluralSeparator* is used as separator between the two strings.

For example : User|Users for key Users

To use plural string, use paramters from [IStringLocalizer](https://github.com/aspnet/AspNetCore/blob/def36fab1e45ef7f169dfe7b59604d0002df3b7c/src/Mvc/Mvc.Localization/src/LocalizedHtmlString.cs), if last parameters is a boolean, pluralization will be activated.


Pluralization is available with IStringLocalizer, IViewLocalizer and HtmlStringLocalizer :

**localizer.GetString("Users", true)**;

# Information

**Platform Support**


## 2.0.0+

|Platform|Version|
| -------------------  | :------------------: |
|NetStandard|2.0+|
|NetCore|2.0.0+|

**WithCulture method**

**WhithCulture** method is not implemented and will not be implemented. ASP.NET Team, start to set this method **Obsolete** for version 3 and will be removed in version 4 of asp.net core.

For more information : 
https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/issues/46

# Performances

After talking with others Devs about my package, they asked my about performance.


``` ini

BenchmarkDotNet=v0.11.5, OS=macOS Mojave 10.14.4 (18E226) [Darwin 18.5.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.106
  [Host]     : .NET Core 2.2.4 (CoreCLR 4.6.27521.02, CoreFX 4.6.27521.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.4 (CoreCLR 4.6.27521.02, CoreFX 4.6.27521.01), 64bit RyuJIT


```
|                                          Method |          Mean |         Error |        StdDev |           Min |           Max |    Ratio | RatioSD |   Gen 0 |   Gen 1 |  Gen 2 | Allocated |
|------------------------------------------------ |--------------:|--------------:|--------------:|--------------:|--------------:|---------:|--------:|--------:|--------:|-------:|----------:|
|                                       Localizer |     109.70 ns |     0.1010 ns |     0.0789 ns |     109.60 ns |     109.89 ns |     1.00 |    0.00 |       - |       - |      - |         - |
|                                   JsonLocalizer |      80.34 ns |     0.2115 ns |     0.1875 ns |      79.99 ns |      80.70 ns |     0.73 |    0.00 |  0.0228 |       - |      - |      48 B |
|                       JsonLocalizerWithCreation | 510,920.91 ns | 2,157.3470 ns | 1,912.4319 ns | 507,912.26 ns | 514,646.87 ns | 4,659.68 |   16.18 | 83.0078 | 27.3438 | 4.8828 |  175576 B |
| JsonLocalizerWithCreationAndExternalMemoryCache |   4,581.21 ns |    12.6355 ns |    11.2011 ns |   4,562.24 ns |   4,605.28 ns |    41.75 |    0.11 |  1.6174 |  0.8087 |      - |    3408 B |
|                JsonLocalizerDefaultCultureValue |     322.22 ns |     0.9659 ns |     0.8563 ns |     320.33 ns |     323.33 ns |     2.94 |    0.01 |  0.1793 |       - |      - |     376 B |
|                    LocalizerDefaultCultureValue |     362.96 ns |     1.8632 ns |     1.5558 ns |     361.14 ns |     365.80 ns |     3.31 |    0.01 |  0.1559 |       - |      - |     328 B |


# Contributors

[@lethek](https://github.com/lethek) : 
- [#20](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/pull/20)
- [#17](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/pull/17)

[@lugospod](https://github.com/lugospod) :
- [#43](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/pull/43)
- [#44](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/pull/44)

[@Compufreak345](https://github.com/Compufreak345) :
- [#52](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/issues/52)

# License

[MIT Licence](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/blob/master/LICENSE)
