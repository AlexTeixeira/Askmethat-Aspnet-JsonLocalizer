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

A extension method is available for `IServiceCollection`.

## JSON Files

By default the code will look for all JSON file inside this directory : `$"{_env.WebRootPath}/Resources/"`;

You can customize the path. You are free to name your file, they just should have the JSON extension

``` cs
//With path
services.AddJsonLocalization(options => options.ResourcesPath = "mypath");
//Wihtout path
services.AddJsonLocalization();
```

## Memory Cache

By default a memory cache of 30 minutes is set to avoid loading all files each needed time.

You can customize this cache duration using a *TimeSpan* like that : 
``` cs
services.AddJsonLocalization(options => options.CacheDuration = TimeSpan.FromMinutes(15));
```

# Informations

**Platform Support**

|Platform|Version|
| -------------------  | :------------------: |
|NetStandard|1.1.3+|
|NetCore|1.1.0+|

# License

[MIT Licence](https://github.com/AlexTeixeira/Askmethat-Aspnet-JsonLocalizer/blob/master/LICENSE)
