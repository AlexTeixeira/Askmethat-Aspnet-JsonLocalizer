
BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
Frequency=3328125 Hz, Resolution=300.4695 ns, Timer=TSC
.NET Core SDK=2.2.103
  [Host]     : .NET Core 2.2.1 (CoreCLR 4.6.27207.03, CoreFX 4.6.27207.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.2.1 (CoreCLR 4.6.27207.03, CoreFX 4.6.27207.03), 64bit RyuJIT


                                          Method |          Mean |         Error |        StdDev |           Min |           Max |     Ratio |  RatioSD |   Gen 0 |   Gen 1 |  Gen 2 | Allocated |
------------------------------------------------ |--------------:|--------------:|--------------:|--------------:|--------------:|----------:|---------:|--------:|--------:|-------:|----------:|
                                   JsonLocalizer |      55.49 ns |      1.176 ns |      2.454 ns |      52.55 ns |      61.80 ns |      1.00 |     0.00 |  0.0114 |       - |      - |      48 B |
                                       Localizer |      98.09 ns |      1.176 ns |      1.042 ns |      96.39 ns |     100.23 ns |      1.78 |     0.06 |       - |       - |      - |         - |
                       JsonLocalizerWithCreation | 647,389.65 ns | 19,441.235 ns | 56,402.548 ns | 573,985.62 ns | 814,434.86 ns | 11,620.42 | 1,268.33 | 41.0156 | 20.5078 | 1.9531 |  174688 B |
 JsonLocalizerWithCreationAndExternalMemoryCache |   5,120.16 ns |     99.768 ns |    152.356 ns |   4,962.78 ns |   5,522.11 ns |     91.02 |     5.12 |  0.9384 |  0.4654 |      - |    3944 B |
                            JsonLocalizerDefault |     268.60 ns |      1.787 ns |      1.492 ns |     266.84 ns |     271.32 ns |      4.88 |     0.15 |  0.0892 |       - |      - |     376 B |
                                LocalizerDefault |     316.64 ns |      4.932 ns |      4.614 ns |     311.53 ns |     326.27 ns |      5.73 |     0.19 |  0.0777 |       - |      - |     328 B |
