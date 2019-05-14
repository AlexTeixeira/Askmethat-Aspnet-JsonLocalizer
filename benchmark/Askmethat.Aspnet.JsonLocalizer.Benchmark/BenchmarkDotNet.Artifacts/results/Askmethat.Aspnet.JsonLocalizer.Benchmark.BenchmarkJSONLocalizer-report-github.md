``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328128 Hz, Resolution=300.4692 ns, Timer=TSC
.NET Core SDK=2.2.103
  [Host] : .NET Core 2.2.1 (CoreCLR 4.6.27207.03, CoreFX 4.6.27207.03), 64bit RyuJIT


```
|                                          Method | Mean | Error | Min | Max | Ratio | RatioSD |
|------------------------------------------------ |-----:|------:|----:|----:|------:|--------:|
|                                       Localizer |   NA |    NA |  NA |  NA |     ? |       ? |
|                                   JsonLocalizer |   NA |    NA |  NA |  NA |     ? |       ? |
|                       JsonLocalizerWithCreation |   NA |    NA |  NA |  NA |     ? |       ? |
| JsonLocalizerWithCreationAndExternalMemoryCache |   NA |    NA |  NA |  NA |     ? |       ? |
|                JsonLocalizerDefaultCultureValue |   NA |    NA |  NA |  NA |     ? |       ? |
|                    LocalizerDefaultCultureValue |   NA |    NA |  NA |  NA |     ? |       ? |

Benchmarks with issues:
  BenchmarkJSONLocalizer.Localizer: DefaultJob
  BenchmarkJSONLocalizer.JsonLocalizer: DefaultJob
  BenchmarkJSONLocalizer.JsonLocalizerWithCreation: DefaultJob
  BenchmarkJSONLocalizer.JsonLocalizerWithCreationAndExternalMemoryCache: DefaultJob
  BenchmarkJSONLocalizer.JsonLocalizerDefaultCultureValue: DefaultJob
  BenchmarkJSONLocalizer.LocalizerDefaultCultureValue: DefaultJob
