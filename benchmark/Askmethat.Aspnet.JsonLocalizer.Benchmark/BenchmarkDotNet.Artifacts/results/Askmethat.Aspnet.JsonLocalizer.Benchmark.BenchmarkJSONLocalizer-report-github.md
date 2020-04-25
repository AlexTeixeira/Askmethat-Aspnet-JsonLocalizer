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
