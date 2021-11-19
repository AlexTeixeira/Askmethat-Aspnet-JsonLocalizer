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
- **CacheDuration** : _Default value : 30 minutes_. We cache all values to memory to avoid loading files for each request, this parameter defines the time after which the cache is refreshed.
- **FileEncoding** : _default value : UTF8_. Specify the file encoding.
- **IsAbsolutePath** : *_default value : false*. Look for an absolute path instead of project path.
- **UseBaseName** : *_default value : false*. Use base name location for Views and constructors like default Resx localization in **ResourcePathFolder**. Please have a look at the documentation below to see the different possiblities for structuring your translation files.
- **Caching** : *_default value: MemoryCache*. Internal caching can be overwritted by using custom class that extends IMemoryCache.
- **PluralSeparator** : *_default value: |*. Seperator used to get singular or pluralized version of localization. More information in *Pluralization*
- **MissingTranslationLogBehavior** : *_default value: LogConsoleError*. Define the logging mode
- **LocalizationMode** : *_default value: Basic*. Define the localization mode for the Json file. Currently Basic and I18n. More information in *LocalizationMode*
- **MissingTranslationsOutputFile** : This enables to specify in which file the missing translations will be written when `MissingTranslationLogBehavior = MissingTranslationLogBehavior.CollectToJSON`, defaults to `MissingTranslations.json`

### Search patterns when UseBaseName = true

If UseBaseName is set to true, it will be searched for lingualization files by the following order - skipping the options below if any option before matches.

- If you use a non-typed IStringLocalizer all files in the Resources-directory, including all subdirectories, will be used to find a localization. This can cause unpredictable behavior if the same key is used in multiple files.

- If you use a typed localizer, the following applies - Namespace is the "short namespace" without the root namespace:
  - Nested classes will use the translation file of their parent class.
  - If there is a folder named "Your/Namespace/And/Classname", all contents of this folder will be used.
  - If there is a folder named "Your/Namespace" the folder will be searched for all json-files beginning with your classname.
  - Otherwise there will be searched for a json-file starting with "Your.Namespace.And.Classname" in your Resources-folder.
  - If there any _.shared.json_ file at base path, all the keys that do not exist in other files will be added. 

- If you need a base shared files, just add a file named _localization.shared.json_ in your **ResourcesPath**

### Pluralization

In version 2.0.0, Pluralization was introduced.
You are now able to manage a singular (left) and plural (right) version for the same Key. 
*PluralSeparator* is used as separator between the two strings.

For example : User|Users for key Users

To use plural string, use paramters from [IStringLocalizer](https://github.com/aspnet/AspNetCore/blob/def36fab1e45ef7f169dfe7b59604d0002df3b7c/src/Mvc/Mvc.Localization/src/LocalizedHtmlString.cs), if last parameters is a boolean, pluralization will be activated.

Pluralization is available with IStringLocalizer, IViewLocalizer and HtmlStringLocalizer :

In version 3.1.1 and above you can have multiple pluralization, to use it, you should 
use IJsonStringLocalizer interface and this method ```LocalizedString GetPlural(string key, double count, params object[] arguments)```

**localizer.GetString("Users", true)**;

### Clean Memory Cache

Version 2.2.0+ allows you to clean cache. 
It's usefull when you want's tu update in live some translations.

**Example**

``` cs
public class HomeController{
  private readonly IJsonStringLocalizer _localizer;
  
  public HomeController(IJsonStringLocalizer<HomeController> localizer)
  {
      _localizer = localizer;
      _localizer.ClearMemCache(new List<CultureInfo>()
      {
          new CultureInfo("en-US")
      });
  }
}
```

# Blazor Server HTML parsing

As you know, Blazor Server does not provide IHtmlLocalizer. To avoid this, you can now use 
from **IJsonStringLocalizer** this method ```MarkupString GetHtmlBlazorString(string name, bool shouldTryDefaultCulture = true)```

# Information

**Platform Support**


|Platform|Version|
| -------------------  | :------------------: |
|NetCore|3.0.0+|
|NetStandard|2.1.0+|
|Blazor Server|3.0.0+|


**WithCulture method**

**WhithCulture** method is not implemented and will not be implemented. ASP.NET Team, start to set this method **Obsolete** for version 3 and will be removed in version 4 of asp.net core.

For more information : 
https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/issues/46

# Localization mode

As asked on the request #64, Some user want to have the possiblities to manage file with i18n way. 
To answer this demand, a localization mode was introduced with default value Basic. Basic version means the the one describe in the previous parts

## I18n

To use the i18n file management, use the the option Localization mode like this : ``` cs LocalizationMode = LocalizationMode.I18n ```.
After that, you should be able to use this json : 

``` json
{
   "Name": "Name",
   "Color": "Color"
}
```

**File name**

File name are important for some purpose (Culture looking, parent culture, fallback).

Please use this pattern : **[fileName].[culture].json**
If you need a fallback culture that target all culture, you can create a file named  **localisation.json**. Of course, if this file does not exist, the chosen default culture is the fallback.

**Important: In this mode, the UseBaseName options should be False.**


For more information : 
https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/issues/64

# Performances

After talking with others Devs about my package, they asked my about performance.


``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15.4 (19E287) [Darwin 19.4.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.1.101
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  DefaultJob : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT


```
|                                          Method |          Mean |        Error |       StdDev |           Min |           Max |    Ratio | RatioSD |   Gen 0 |   Gen 1 |  Gen 2 | Allocated |
|------------------------------------------------ |--------------:|-------------:|-------------:|--------------:|--------------:|---------:|--------:|--------:|--------:|-------:|----------:|
|                                       Localizer |     108.02 ns |     0.240 ns |     0.187 ns |     107.74 ns |     108.44 ns |     1.00 |    0.00 |       - |       - |      - |         - |
|                                   JsonLocalizer |      72.37 ns |     0.422 ns |     0.352 ns |      72.08 ns |      73.19 ns |     0.67 |    0.00 |  0.0229 |       - |      - |      48 B |
|                       JsonLocalizerWithCreation | 177,880.37 ns | 2,095.010 ns | 1,959.674 ns | 175,159.17 ns | 181,548.27 ns | 1,645.06 |   16.23 | 26.3672 | 13.1836 | 0.9766 |   55231 B |
|                   I18nJsonLocalizerWithCreation | 257,434.74 ns | 1,743.759 ns | 1,545.797 ns | 255,822.30 ns | 260,643.68 ns | 2,384.22 |   13.97 | 45.4102 | 22.4609 | 1.4648 |   95942 B |
| JsonLocalizerWithCreationAndExternalMemoryCache |   3,951.97 ns |    33.265 ns |    29.488 ns |   3,909.51 ns |   4,010.62 ns |    36.54 |    0.22 |  1.4954 |  0.7477 |      - |    3137 B |
|                JsonLocalizerDefaultCultureValue |     256.51 ns |     2.619 ns |     2.322 ns |     254.03 ns |     261.52 ns |     2.37 |    0.02 |  0.1335 |       - |      - |     280 B |
|                    LocalizerDefaultCultureValue |     277.03 ns |     0.889 ns |     0.694 ns |     276.42 ns |     278.72 ns |     2.56 |    0.01 |  0.1030 |       - |      - |     216 B |


# Contributors

<table>
  <tr>
    <td align="center">
      <a href="https://github.com/lethek">
        <img src="https://avatars2.githubusercontent.com/u/52574?s=460&v=4" width="100px;" alt="Michael Monsour"/>
        <br />
        <sub><b>Michael Monsour</b></sub>
      </a>
    </td>
     <td align="center">
      <a href="https://github.com/lugospod">
        <img src="https://avatars1.githubusercontent.com/u/29342608?s=460&v=4" width="100px;" alt="Luka Gospodnetic"/>
        <br />
        <sub><b>Luka Gospodnetic</b></sub>
      </a>
    </td>
     <td align="center">
      <a href="https://github.com/Compufreak345">
        <img src="https://avatars3.githubusercontent.com/u/10026694?s=460&v=4" width="100px;" alt="Christoph Sonntag"/>
        <br />
        <sub><b>Christoph Sonntag</b></sub>
      </a>
    </td>
     <td align="center">
      <a href="https://github.com/Dunning-Kruger">
        <img src="https://avatars0.githubusercontent.com/u/1564825?s=460&v=4" width="100px;" alt="Nacho"/>
        <br />
        <sub><b>Nacho</b></sub>
      </a>
    </td>
     <td align="center">
      <a href="https://github.com/AshleyMedway">
        <img src="https://avatars3.githubusercontent.com/u/1255596?s=460&v=4" width="100px;" alt="Ashley Medway"/>
        <br />
        <sub><b>Ashley Medway</b></sub>
      </a>
    </td>
     <td align="center">
      <a href="https://github.com/NoPasaran0218">
        <img src="https://avatars2.githubusercontent.com/u/25226807?s=460&v=4" width="100px;" alt="Serhii Voitovych"/>
        <br />
        <sub><b>Serhii Voitovych</b></sub>
      </a>
    </td>
    <td align="center">
        <a href="https://github.com/JamesHill3">
            <img src="https://avatars0.githubusercontent.com/u/1727474?s=460&v=4" width="100px;" alt="James Hill"/>
            <br />
            <sub><b>James Hill</b></sub>
        </a>
    </td>
    <td align="center">
        <a href="https://github.com/Czirok">
            <img src="https://avatars2.githubusercontent.com/u/1266377?s=460&v=4" width="100px;" alt="Ferenc Czirok"/>
            <br />
            <sub><b>Ferenc Czirok</b></sub>
        </a>
    </td>
     <td align="center">
            <a href="https://github.com/rohanreddyg">
                <img src="https://avatars0.githubusercontent.com/u/240114?s=400&v=4" width="100px;" alt="rohanreddyg"/>
                <br />
                <sub><b>rohanreddyg</b></sub>
            </a>
        </td>
 <td align="center">
            <a href="https://github.com/rickszyr">
                <img src="https://avatars.githubusercontent.com/u/10763102?s=460&v=4" width="100px;" alt="rickszyr"/>
                <br />
                <sub><b>rickszyr</b></sub>
            </a>
        </td>
 <td align="center">
            <a href="https://github.com/ErikApption">
                <img src="https://avatars.githubusercontent.com/u/3179656?s=460&u=4a6b52f80b64f5951d3d04b4cfe18ac7f050a52a&v=4" width="100px;" alt="ErikApption"/>
                <br />
                <sub><b>ErikApption</b></sub>
            </a>
        </td>
  </tr>
  
</table>

A special thanks to @Compufreak345 for its hard work. He did a lot for this repo.

# License

[MIT Licence](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/blob/master/LICENSE)
