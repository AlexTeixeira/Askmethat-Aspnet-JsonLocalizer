``` ini

BenchmarkDotNet=v0.11.5, OS=macOS 10.15 (19A583) [Darwin 19.0.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.0.100
  [Host]     : .NET Core 2.2.5 (CoreCLR 4.6.27617.05, CoreFX 4.6.27618.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.5 (CoreCLR 4.6.27617.05, CoreFX 4.6.27618.01), 64bit RyuJIT


```
|                                          Method |          Mean |        Error |       StdDev |        Median |           Min |           Max |    Ratio | RatioSD |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------------------------------------------------ |--------------:|-------------:|-------------:|--------------:|--------------:|--------------:|---------:|--------:|--------:|--------:|--------:|----------:|
|                                       Localizer |     119.53 ns |     2.884 ns |     7.235 ns |     116.33 ns |     115.35 ns |     146.81 ns |     1.00 |    0.00 |       - |       - |       - |         - |
|                                   JsonLocalizer |      89.11 ns |     1.800 ns |     2.277 ns |      87.98 ns |      86.93 ns |      94.85 ns |     0.72 |    0.05 |  0.0229 |       - |       - |      48 B |
|                       JsonLocalizerWithCreation | 596,524.59 ns | 4,863.030 ns | 4,310.949 ns | 595,607.83 ns | 591,494.76 ns | 606,305.87 ns | 4,828.78 |  382.40 | 91.7969 | 44.9219 | 21.4844 |  192847 B |
| JsonLocalizerWithCreationAndExternalMemoryCache |   4,863.67 ns |    79.603 ns |    74.461 ns |   4,852.63 ns |   4,773.70 ns |   5,034.50 ns |    39.52 |    2.99 |  1.6022 |  0.8011 |  0.0229 |    3368 B |
|                JsonLocalizerDefaultCultureValue |     342.37 ns |     1.795 ns |     1.499 ns |     342.30 ns |     340.15 ns |     344.83 ns |     2.78 |    0.22 |  0.1793 |       - |       - |     376 B |
|                    LocalizerDefaultCultureValue |     373.28 ns |     1.421 ns |     1.259 ns |     373.14 ns |     371.53 ns |     376.51 ns |     3.02 |    0.23 |  0.1559 |       - |       - |     328 B |
