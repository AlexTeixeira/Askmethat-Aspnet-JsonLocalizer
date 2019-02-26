
BenchmarkDotNet=v0.11.3, OS=macOS Mojave 10.14.3 (18D109) [Darwin 18.2.0]
Intel Core i7-5557U CPU 3.10GHz (Broadwell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.2.100
  [Host]     : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT


        Method |      Mean |     Error |    StdDev |       Min |       Max | Ratio | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
-------------- |----------:|----------:|----------:|----------:|----------:|------:|------------:|------------:|------------:|--------------------:|
 JsonLocalizer |  78.70 ns | 0.4997 ns | 0.4430 ns |  77.91 ns |  79.50 ns |  0.66 |      0.0229 |           - |           - |                48 B |
     Localizer | 119.94 ns | 0.4889 ns | 0.4083 ns | 119.48 ns | 120.76 ns |  1.00 |           - |           - |           - |                   - |
