``` ini

BenchmarkDotNet=v0.12.0, OS=macOS 10.15.4 (19E287) [Darwin 19.4.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.1.101
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  DefaultJob : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT


```
|                                          Method |          Mean |        Error |       StdDev |           Min |           Max |    Ratio | RatioSD |   Gen 0 |   Gen 1 |  Gen 2 | Allocated |
|------------------------------------------------ |--------------:|-------------:|-------------:|--------------:|--------------:|---------:|--------:|--------:|--------:|-------:|----------:|
|                                       Localizer |     106.92 ns |     1.939 ns |     1.719 ns |     104.82 ns |     111.35 ns |     1.00 |    0.00 |       - |       - |      - |         - |
|                                   JsonLocalizer |      72.25 ns |     0.388 ns |     0.363 ns |      71.84 ns |      73.14 ns |     0.68 |    0.01 |  0.0229 |       - |      - |      48 B |
|                       JsonLocalizerWithCreation | 181,926.57 ns | 1,810.931 ns | 1,605.343 ns | 180,374.02 ns | 185,809.62 ns | 1,701.93 |   34.00 | 26.1230 | 12.9395 | 4.3945 |   54999 B |
|                   I18nJsonLocalizerWithCreation | 260,527.13 ns | 1,295.787 ns | 1,082.040 ns | 258,829.38 ns | 262,368.86 ns | 2,436.50 |   43.15 | 45.4102 | 22.4609 | 7.8125 |   95644 B |
| JsonLocalizerWithCreationAndExternalMemoryCache |   4,172.26 ns |    77.594 ns |    72.582 ns |   4,092.12 ns |   4,300.93 ns |    39.08 |    0.93 |  1.4954 |  0.7477 | 0.0229 |    3136 B |
|                JsonLocalizerDefaultCultureValue |     249.02 ns |     2.381 ns |     2.111 ns |     246.74 ns |     253.38 ns |     2.33 |    0.04 |  0.1335 |       - |      - |     280 B |
|                    LocalizerDefaultCultureValue |     280.40 ns |     3.926 ns |     3.673 ns |     277.42 ns |     289.81 ns |     2.62 |    0.06 |  0.1030 |       - |      - |     216 B |
