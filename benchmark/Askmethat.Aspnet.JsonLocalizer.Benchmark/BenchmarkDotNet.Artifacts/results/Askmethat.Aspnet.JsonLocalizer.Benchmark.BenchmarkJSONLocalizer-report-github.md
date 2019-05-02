``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328125 Hz, Resolution=300.4695 ns, Timer=TSC
.NET Core SDK=2.2.103
  [Host]     : .NET Core 2.2.1 (CoreCLR 4.6.27207.03, CoreFX 4.6.27207.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.1 (CoreCLR 4.6.27207.03, CoreFX 4.6.27207.03), 64bit RyuJIT


```
|                                          Method |          Mean |         Error |        StdDev |        Median |           Min |           Max |    Ratio | RatioSD |   Gen 0 |   Gen 1 |  Gen 2 | Allocated |
|------------------------------------------------ |--------------:|--------------:|--------------:|--------------:|--------------:|--------------:|---------:|--------:|--------:|--------:|-------:|----------:|
|                                   JsonLocalizer |      85.36 ns |      1.777 ns |      2.247 ns |      84.88 ns |      82.71 ns |      90.84 ns |     1.00 |    0.00 |  0.0113 |       - |      - |      48 B |
|                                       Localizer |     111.95 ns |      5.681 ns |     16.482 ns |     103.47 ns |      97.57 ns |     154.47 ns |     1.33 |    0.13 |       - |       - |      - |         - |
|                       JsonLocalizerWithCreation | 679,279.52 ns | 14,495.178 ns | 42,511.848 ns | 679,310.74 ns | 603,370.89 ns | 779,815.73 ns | 7,837.87 |  594.65 | 41.0156 | 20.5078 | 1.9531 |  174232 B |
| JsonLocalizerWithCreationAndExternalMemoryCache |   5,356.52 ns |    267.907 ns |    764.352 ns |   5,242.24 ns |   4,344.78 ns |   7,235.60 ns |    72.62 |   10.30 |  0.8392 |  0.4196 |      - |    3536 B |
|                JsonLocalizerDefaultCultureValue |     329.43 ns |      6.936 ns |     20.012 ns |     333.79 ns |     262.30 ns |     376.34 ns |     3.96 |    0.24 |  0.0892 |       - |      - |     376 B |
|                    LocalizerDefaultCultureValue |     377.61 ns |     16.365 ns |     47.997 ns |     359.06 ns |     323.90 ns |     492.92 ns |     4.29 |    0.35 |  0.0777 |       - |      - |     328 B |
