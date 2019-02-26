``` ini

BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14.3 (18D109) [Darwin 18.2.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.100
  [Host]     : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT


```
|        Method |      Mean |     Error |    StdDev |       Min |       Max | Ratio | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|-------------- |----------:|----------:|----------:|----------:|----------:|------:|------------:|------------:|------------:|--------------------:|
| JsonLocalizer |  72.69 ns | 0.2973 ns | 0.2483 ns |  72.35 ns |  73.34 ns |  0.57 |      0.0228 |           - |           - |                48 B |
|     Localizer | 126.99 ns | 1.8120 ns | 1.6950 ns | 124.92 ns | 130.10 ns |  1.00 |           - |           - |           - |                   - |
