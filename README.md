# Askmethat-Aspnet-JsonLocalizer
Json Localizer library for .NetStandard and .NetCore Asp.net projects

#### Nuget
[![NuGet](https://img.shields.io/nuget/dt/Askmethat.Aspnet.JsonLocalizer.svg)](https://www.nuget.org/packages/Askmethat.Aspnet.JsonLocalizer)

#### Build

[![Build status](https://ci.appveyor.com/api/projects/status/gt8vg0e2f9gapr2d/branch/master?svg=true)](https://ci.appveyor.com/project/AlexTeixeira/askmethat-aspnet-jsonlocalizer/branch/master)

# Project

This library allow user to use JSON files instead of RESX in Asp.net application.
You can have several files in one folder. This allows you to better manage your translations according to your preferences.
The library is compatible with NetStandard & NetCore

# Configuration

An extension method is available for `IServiceCollection`.


# Breaking Changes

For performance purpose, JSON structure was changes since 2.0.0 from List to Dictionnary.
So if you move to 1.1.7 or less, please donc forget to change your JSON files.

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

A set of options is available, and you can set it when you add JsonLocalization to the your Services.

``` cs
services.AddJsonLocalization(options => {
        options.CacheDuration = TimeSpan.FromMinutes(15);
        options.ResourcesPath = "mypath";
        options.FileEncoding = Encoding.GetEncoding("ISO-8859-1");
    });
```

### Current Options

- **ResourcesPath** : _Default value : `$"{_env.WebRootPath}/Resources/"`_.  Base path of your resources. The plugin will browse the folder and sub-folders and load all present JSON files.
- **CacheDuration** : _Default value : 30 minutes_. Cache all values to memory to avoid loading files for each request,
- **FileEncoding** : _default value : UTF8_. Specify the file encoding.
- **IsAbsolutePath** : *_default value : false*. Look for an absolute path instead of project path.
- **UseBaseName** : *_default value : false*. Use base name location for Views and consors like default Resx localization in **ResourcePathFolder**.
- **Caching** : *_default value: MemoryCache*. Internal caching can be overwritted by using custom class that extends IMemoryCache.
- **PluralSeparator** : *_default value: |*. Seperator used to get singular or pluralized version of localization. More information in *Pluralization*

#Pluralization

In version 2.0.0, Pluralization was introduced to be able to manage a singular (left) and plural (rigth) version for the same Key. *PluralSeparator* is used as separator between the two string.

For example : User|Users for key Users

To use plural string you only need to set last parameter as boolean in GetString methods 
with IStringLocalizer and IViewLocalizer:

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

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14.3 (18D109) [Darwin 18.2.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.100
  [Host]     : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT


```
|        Method |      Mean |     Error |    StdDev |       Min |       Max | Ratio | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|-------------- |----------:|----------:|----------:|----------:|----------:|------:|------------:|------------:|------------:|--------------------:|
| JsonLocalizer |  72.69 ns | 0.2973 ns | 0.2483 ns |  72.35 ns |  73.34 ns |  0.57 |      0.0228 |           - |           - |                48 B |
|     Localizer | 126.99 ns | 1.8120 ns | 1.6950 ns | 124.92 ns | 130.10 ns |  1.00 |           - |           - |           - |                   - |


# Contributors

[@lethek](https://github.com/lethek) : PRs : [#20](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/pull/20), [#17](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/pull/17)

# License

[MIT Licence](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/blob/master/LICENSE)
