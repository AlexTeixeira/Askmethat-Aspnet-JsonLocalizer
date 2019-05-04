# Askmethat-Aspnet-JsonLocalizer
Json Localizer library for .NetStandard and .NetCore Asp.net projects

#### Nuget
[![NuGet](https://img.shields.io/nuget/dt/Askmethat.Aspnet.JsonLocalizer.svg)](https://www.nuget.org/packages/Askmethat.Aspnet.JsonLocalizer)

#### Build

[![Build status](https://ci.appveyor.com/api/projects/status/gt8vg0e2f9gapr2d/branch/master?svg=true)](https://ci.appveyor.com/project/AlexTeixeira/askmethat-aspnet-jsonlocalizer/branch/master)

# Project

This library allow user to use JSON files instead of RESX in Asp.net application.
The code try to be most compliante with Microsoft guidelines.
The library is compatible with NetStandard & NetCore

# Configuration

An extension method is available for `IServiceCollection`.
You can have a look to this method [here](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/blob/development/Askmethat.Aspnet.JsonLocalizer/Extensions/JsonLocalizerServiceExtension.cs)

# Breaking Changes

For performance purpose, JSON structure was changes since 2.0.0 from List to Dictionnary.
Here the detail for version before 1.1.7 and version after 2.0.0

## 1.1.7-

``` json
[
  {
    "Key": "Name3",
    "Values": {
      "en-US": "My Name 3",
      "fr-FR": "Mon Nom 3"
    }
  }
]
```

## 2.0.0+

``` json
{
  "Name3": {
    "Values": {
      "en-US": "My Name 3",
      "fr-FR": "Mon Nom 3"
    }
  }
}
```

## Options 

A set of options is available.
You can define them like this : 

``` cs
services.AddJsonLocalization(options => {
        options.CacheDuration = TimeSpan.FromMinutes(15);
        options.ResourcesPath = "mypath";
        options.FileEncoding = Encoding.GetEncoding("ISO-8859-1");
        options.SupportedCultureInfos = new[]
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
You are now able to manage a singular (left) and plural (rigth) version for the same Key. 
*PluralSeparator* is used as separator between the two strings.

For example : User|Users for key Users

To use plural string, use paramters from [IStringLocalizer](https://github.com/aspnet/AspNetCore/blob/def36fab1e45ef7f169dfe7b59604d0002df3b7c/src/Mvc/Mvc.Localization/src/LocalizedHtmlString.cs), if last parameters is a boolean, pluralization will be activated.


Pluralization is available with IStringLocalizer, IViewLocalizer and HtmlStringLocalizer :

**localizer.GetString("Users", true)**;

# Information

**Platform Support**

## 1.1.7

|Platform|Version|
| -------------------  | :------------------: |
|NetStandard|1.1.6+|
|NetCore|2.0.0+|

## 2.0.0+

|Platform|Version|
| -------------------  | :------------------: |
|NetStandard|2.0+|
|NetCore|2.0.0+|

**WithCulture method**

**WhithCulture** method is not implemented and will be not implemented. ASP.NET Team, start to set this method **Obsolete** fr version 3 and will be removed in version 4 of asp.net core.

For more information : 
https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/issues/46

# Performances

After talking with others Devs about my package, they ask my about performance.

So I added a benchmark project and here the last results with some modification, that will be available with 1.1.7

## 1.1.7

``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14 (18A391) [Darwin 18.0.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.100
  [Host]     : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT


```
|        Method |     Mean |     Error |    StdDev |      Min |      Max | Ratio | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|-------------- |---------:|----------:|----------:|---------:|---------:|------:|------------:|------------:|------------:|--------------------:|
| JsonLocalizer | 255.1 ns | 0.7950 ns | 0.7048 ns | 253.3 ns | 256.4 ns |  2.18 |      0.0648 |           - |           - |               136 B |
|     Localizer | 117.2 ns | 0.2544 ns | 0.2255 ns | 116.8 ns | 117.5 ns |  1.00 |           - |           - |           - |                   - |


## 2.0.0+

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

# License

[MIT Licence](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/blob/master/LICENSE)
