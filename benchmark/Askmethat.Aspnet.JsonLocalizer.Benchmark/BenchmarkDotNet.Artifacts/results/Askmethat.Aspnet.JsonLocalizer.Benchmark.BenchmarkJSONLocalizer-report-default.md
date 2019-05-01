
BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14.4 (18E226) [Darwin 18.5.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.106
  [Host]     : .NET Core 2.2.4 (CoreCLR 4.6.27521.02, CoreFX 4.6.27521.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.4 (CoreCLR 4.6.27521.02, CoreFX 4.6.27521.01), 64bit RyuJIT


                                          Method |          Mean |         Error |        StdDev |           Min |           Max |    Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
------------------------------------------------ |--------------:|--------------:|--------------:|--------------:|--------------:|---------:|--------:|------------:|------------:|------------:|--------------------:|
                                   JsonLocalizer |      68.83 ns |     0.1259 ns |     0.1116 ns |      68.58 ns |      68.95 ns |     0.63 |    0.00 |      0.0228 |           - |           - |                48 B |
                       JsonLocalizerWithCreation | 512,929.54 ns | 1,795.6669 ns | 1,679.6679 ns | 510,100.03 ns | 515,499.84 ns | 4,678.08 |   13.62 |     83.0078 |     29.2969 |      4.8828 |            174968 B |
 JsonLocalizerWithCreationAndExternalMemoryCache |   4,930.42 ns |    20.7277 ns |    18.3746 ns |   4,888.34 ns |   4,956.98 ns |    44.98 |    0.18 |      1.7624 |      0.8774 |           - |              3712 B |
                                       Localizer |     109.62 ns |     0.1763 ns |     0.1563 ns |     109.48 ns |     110.05 ns |     1.00 |    0.00 |           - |           - |           - |                   - |
