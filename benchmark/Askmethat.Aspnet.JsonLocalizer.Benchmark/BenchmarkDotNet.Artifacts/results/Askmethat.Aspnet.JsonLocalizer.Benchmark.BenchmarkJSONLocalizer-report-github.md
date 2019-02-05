``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14 (18A391) [Darwin 18.0.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.100
  [Host]     : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT


```
|        Method |     Mean |     Error |    StdDev |      Min |      Max | Ratio | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|-------------- |---------:|----------:|----------:|---------:|---------:|------:|------------:|------------:|------------:|--------------------:|
| JsonLocalizer | 255.1 ns | 0.7950 ns | 0.7048 ns | 253.3 ns | 256.4 ns |  2.18 |      0.0648 |           - |           - |               136 B |
|     Localizer | 117.2 ns | 0.2544 ns | 0.2255 ns | 116.8 ns | 117.5 ns |  1.00 |           - |           - |           - |                   - |
