
BenchmarkDotNet=v0.11.5, OS=macOS Mojave 10.14.4 (18E226) [Darwin 18.5.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.106
  [Host]     : .NET Core 2.2.4 (CoreCLR 4.6.27521.02, CoreFX 4.6.27521.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.4 (CoreCLR 4.6.27521.02, CoreFX 4.6.27521.01), 64bit RyuJIT


                                          Method |          Mean |         Error |        StdDev |           Min |           Max |    Ratio | RatioSD |   Gen 0 |   Gen 1 |  Gen 2 | Allocated |
------------------------------------------------ |--------------:|--------------:|--------------:|--------------:|--------------:|---------:|--------:|--------:|--------:|-------:|----------:|
                                       Localizer |     115.89 ns |     0.1277 ns |     0.1132 ns |     115.71 ns |     116.03 ns |     1.00 |    0.00 |       - |       - |      - |         - |
                                   JsonLocalizer |      80.46 ns |     0.0964 ns |     0.0805 ns |      80.37 ns |      80.61 ns |     0.69 |    0.00 |  0.0228 |       - |      - |      48 B |
                       JsonLocalizerWithCreation | 533,754.90 ns | 3,074.1865 ns | 2,875.5960 ns | 529,323.89 ns | 539,354.61 ns | 4,606.83 |   25.03 | 83.0078 | 28.3203 | 3.9063 |  175880 B |
 JsonLocalizerWithCreationAndExternalMemoryCache |   4,734.27 ns |   135.6678 ns |   133.2439 ns |   4,643.43 ns |   5,162.57 ns |    40.91 |    1.22 |  1.6174 |  0.8087 |      - |    3408 B |
                JsonLocalizerDefaultCultureValue |     331.54 ns |     1.7528 ns |     1.5539 ns |     329.22 ns |     334.37 ns |     2.86 |    0.01 |  0.1793 |       - |      - |     376 B |
                    LocalizerDefaultCultureValue |     371.86 ns |     3.8521 ns |     3.6033 ns |     368.69 ns |     378.81 ns |     3.20 |    0.03 |  0.1559 |       - |      - |     328 B |
