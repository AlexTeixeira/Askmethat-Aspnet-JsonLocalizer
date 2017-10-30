# Askmethat-Aspnet-JsonLocalizer
Json Localizer library for .NetStandard and .NetCore Asp.net projects

#### Nuget
[![NuGet](https://img.shields.io/nuget/dt/Askmethat.Aspnet.JsonLocalize.svg)](https://www.nuget.org/packages/Askmethat.Aspnet.JsonLocalizer)

#### Build

[![Build status](https://ci.appveyor.com/api/projects/status/gt8vg0e2f9gapr2d/branch/master?svg=true)](https://ci.appveyor.com/project/AlexTeixeira/askmethat-aspnet-jsonlocalizer/branch/master)

# Project

This library allow user to use JSON files instead of RESX in Asp.net application.
The library is compatible with NetStandard & NetCore

# Sample

A extension method is available for `IServiceCollection`.

By default the path for the JSON file is : `$"{_env.WebRootPath}Resources/localization.json"`;

You can customize the path. You should name your file : `localization.json`

``` cs
//With path
services.AddJsonLocalization(options => options.ResourcesPath = "mypath");
//Wihtout path
services.AddJsonLocalization();
```

# Informations

**Platform Support**

|Platform|Version|
| -------------------  | :------------------: |
|NetStandard|2.0.0+|
|NetCore|2.0.0+|

# License

[MIT Licence](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/blob/master/LICENSE)
