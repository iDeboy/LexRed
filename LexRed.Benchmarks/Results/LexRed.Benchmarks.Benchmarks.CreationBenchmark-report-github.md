```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4541/23H2/2023Update/SunValley3)
AMD Ryzen 5 5600G with Radeon Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.100
  [Host]  : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  LongRun : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2

Job=LongRun  IterationCount=100  LaunchCount=3  
WarmupCount=15  

```
| Method                | Regex                | Mean       | Error     | StdDev    | Median     | Allocated |
|---------------------- |--------------------- |-----------:|----------:|----------:|-----------:|----------:|
| **CreateBrzozowskiRegex** | **((a\|b(...)\*t)\*u [56]** |   **2.063 μs** | **0.0108 μs** | **0.0550 μs** |   **2.041 μs** |    **3.1 KB** |
| CreateBrzozowskiDFA   | ((a\|b(...)\*t)\*u [56] |  11.892 μs | 0.0837 μs | 0.4229 μs |  11.744 μs |  15.45 KB |
| CreateThompsonRegex   | ((a\|b(...)\*t)\*u [56] |  17.177 μs | 0.1067 μs | 0.5087 μs |  17.343 μs |  89.23 KB |
| CreateThompsonNFA     | ((a\|b(...)\*t)\*u [56] |  10.820 μs | 0.0190 μs | 0.0966 μs |  10.830 μs |   7.55 KB |
| CreateThompsonDFA     | ((a\|b(...)\*t)\*u [56] |  74.776 μs | 0.1598 μs | 0.7981 μs |  74.763 μs |  98.77 KB |
| **CreateBrzozowskiRegex** | **(a\|b)(...)(a\|b) [27]** |   **1.013 μs** | **0.0035 μs** | **0.0172 μs** |   **1.009 μs** |    **1.7 KB** |
| CreateBrzozowskiDFA   | (a\|b)(...)(a\|b) [27] | 113.002 μs | 0.3559 μs | 1.8037 μs | 113.267 μs |  85.14 KB |
| CreateThompsonRegex   | (a\|b)(...)(a\|b) [27] |   5.141 μs | 0.0356 μs | 0.1693 μs |   5.108 μs |  28.02 KB |
| CreateThompsonNFA     | (a\|b)(...)(a\|b) [27] |   2.053 μs | 0.0053 μs | 0.0265 μs |   2.046 μs |   2.26 KB |
| CreateThompsonDFA     | (a\|b)(...)(a\|b) [27] |  57.047 μs | 0.1341 μs | 0.6597 μs |  56.934 μs |  85.64 KB |
