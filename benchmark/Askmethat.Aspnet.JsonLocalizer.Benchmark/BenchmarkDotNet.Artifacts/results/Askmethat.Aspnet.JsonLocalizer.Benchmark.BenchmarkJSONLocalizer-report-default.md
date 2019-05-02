
BenchmarkDotNet=v0.11.5, OS=macOS Mojave 10.14.4 (18E226) [Darwin 18.5.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.106
  [Host]     : .NET Core 2.2.4 (CoreCLR 4.6.27521.02, CoreFX 4.6.27521.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.4 (CoreCLR 4.6.27521.02, CoreFX 4.6.27521.01), 64bit RyuJIT


                                          Method |          Mean |         Error |        StdDev |           Min |           Max |    Ratio | RatioSD |   Gen 0 |   Gen 1 |  Gen 2 | Allocated |
------------------------------------------------ |--------------:|--------------:|--------------:|--------------:|--------------:|---------:|--------:|--------:|--------:|-------:|----------:|
                                       Localizer |     109.70 ns |     0.1010 ns |     0.0789 ns |     109.60 ns |     109.89 ns |     1.00 |    0.00 |       - |       - |      - |         - |
                                   JsonLocalizer |      80.34 ns |     0.2115 ns |     0.1875 ns |      79.99 ns |      80.70 ns |     0.73 |    0.00 |  0.0228 |       - |      - |      48 B |
                       JsonLocalizerWithCreation | 510,920.91 ns | 2,157.3470 ns | 1,912.4319 ns | 507,912.26 ns | 514,646.87 ns | 4,659.68 |   16.18 | 83.0078 | 27.3438 | 4.8828 |  175576 B |
 JsonLocalizerWithCreationAndExternalMemoryCache |   4,581.21 ns |    12.6355 ns |    11.2011 ns |   4,562.24 ns |   4,605.28 ns |    41.75 |    0.11 |  1.6174 |  0.8087 |      - |    3408 B |
                JsonLocalizerDefaultCultureValue |     322.22 ns |     0.9659 ns |     0.8563 ns |     320.33 ns |     323.33 ns |     2.94 |    0.01 |  0.1793 |       - |      - |     376 B |
                    LocalizerDefaultCultureValue |     362.96 ns |     1.8632 ns |     1.5558 ns |     361.14 ns |     365.80 ns |     3.31 |    0.01 |  0.1559 |       - |      - |     328 B |
