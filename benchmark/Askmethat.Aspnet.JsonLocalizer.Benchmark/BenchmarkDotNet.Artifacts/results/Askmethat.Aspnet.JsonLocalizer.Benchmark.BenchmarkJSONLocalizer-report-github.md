``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14 (18A391) [Darwin 18.0.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.100
  [Host]     : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT


```
|        Method |      Mean |     Error |    StdDev |       Min |       Max | Ratio | RatioSD | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|-------------- |----------:|----------:|----------:|----------:|----------:|------:|--------:|------------:|------------:|------------:|--------------------:|
| JsonLocalizer |  81.74 ns | 2.1128 ns | 2.8205 ns |  78.84 ns |  90.64 ns |  0.70 |    0.03 |      0.0228 |           - |           - |                48 B |
|     Localizer | 118.42 ns | 0.7853 ns | 0.7345 ns | 117.32 ns | 119.70 ns |  1.00 |    0.00 |           - |           - |           - |                   - |
