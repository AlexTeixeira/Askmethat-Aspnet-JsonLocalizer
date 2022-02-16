``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
Intel Core i7-10870H CPU 2.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT


```
|                                          Method |          Mean |        Error |       StdDev |           Min |           Max |    Ratio | RatioSD |   Gen 0 |  Gen 1 |  Gen 2 | Allocated |
|------------------------------------------------ |--------------:|-------------:|-------------:|--------------:|--------------:|---------:|--------:|--------:|-------:|-------:|----------:|
|                                       Localizer |      57.34 ns |     0.590 ns |     0.523 ns |      56.65 ns |      58.46 ns |     1.00 |    0.00 |       - |      - |      - |         - |
|                                   JsonLocalizer |      41.50 ns |     0.552 ns |     0.516 ns |      40.60 ns |      42.46 ns |     0.72 |    0.01 |  0.0057 |      - |      - |      48 B |
|                       JsonLocalizerWithCreation | 169,174.60 ns | 1,070.840 ns | 1,001.664 ns | 167,445.80 ns | 170,873.85 ns | 2,950.03 |   33.21 |  4.6387 | 2.1973 | 0.2441 |  40,706 B |
|                   I18nJsonLocalizerWithCreation | 228,438.65 ns | 4,188.350 ns | 6,643.166 ns | 218,070.12 ns | 245,103.20 ns | 4,026.62 |  130.32 | 12.2070 | 6.1035 | 0.4883 | 104,172 B |
| JsonLocalizerWithCreationAndExternalMemoryCache |   2,813.26 ns |    51.894 ns |    48.541 ns |   2,731.36 ns |   2,920.27 ns |    49.04 |    0.92 |  0.5264 | 0.2632 |      - |   4,424 B |
|                JsonLocalizerDefaultCultureValue |     145.34 ns |     1.284 ns |     1.201 ns |     142.61 ns |     146.81 ns |     2.53 |    0.04 |  0.0315 |      - |      - |     264 B |
|                    LocalizerDefaultCultureValue |     159.06 ns |     0.919 ns |     0.859 ns |     157.63 ns |     160.51 ns |     2.77 |    0.03 |  0.0257 |      - |      - |     216 B |
