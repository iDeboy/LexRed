```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4541/23H2/2023Update/SunValley3)
AMD Ryzen 5 5600G with Radeon Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.100
  [Host]  : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2
  LongRun : .NET 8.0.11 (8.0.1124.51707), X64 RyuJIT AVX2

Job=LongRun  IterationCount=100  LaunchCount=3  
WarmupCount=15  

```
| Method                   | Regex                | Input                | Mean       | Error    | StdDev    | Median     | Allocated |
|------------------------- |--------------------- |--------------------- |-----------:|---------:|----------:|-----------:|----------:|
| **Brzozowski_Regex_IsMatch** | **((a|b(...)*t)*u [56]** | **abcdb(...)sqrtu [21]** | **4,978.7 ns** | **21.58 ns** | **110.55 ns** | **4,925.5 ns** |    **5552 B** |
| Brzozowski_DFA_IsMatch   | ((a|b(...)*t)*u [56] | abcdb(...)sqrtu [21] |   742.7 ns |  9.02 ns |  44.19 ns |   755.1 ns |         - |
| Thompson_NFA_IsMatch     | ((a|b(...)*t)*u [56] | abcdb(...)sqrtu [21] | 4,251.6 ns | 31.74 ns | 154.96 ns | 4,267.7 ns |   10416 B |
| Thompson_DFA_IsMatch     | ((a|b(...)*t)*u [56] | abcdb(...)sqrtu [21] | 2,064.7 ns | 29.78 ns | 150.93 ns | 2,111.9 ns |         - |
| **Brzozowski_Regex_IsMatch** | **((a|b(...)*t)*u [56]** | **bbaababa**             | **1,619.3 ns** | **10.14 ns** |  **51.87 ns** | **1,601.5 ns** |    **1936 B** |
| Brzozowski_DFA_IsMatch   | ((a|b(...)*t)*u [56] | bbaababa             |   308.4 ns |  7.26 ns |  36.53 ns |   320.8 ns |         - |
| Thompson_NFA_IsMatch     | ((a|b(...)*t)*u [56] | bbaababa             | 1,508.6 ns |  8.99 ns |  46.77 ns | 1,490.2 ns |    4296 B |
| Thompson_DFA_IsMatch     | ((a|b(...)*t)*u [56] | bbaababa             |   964.2 ns | 28.31 ns | 145.54 ns |   943.9 ns |         - |
| **Brzozowski_Regex_IsMatch** | **(a|b)(...)(a|b) [27]** | **abcdb(...)sqrtu [21]** |   **864.9 ns** |  **5.56 ns** |  **28.74 ns** |   **876.0 ns** |     **664 B** |
| Brzozowski_DFA_IsMatch   | (a|b)(...)(a|b) [27] | abcdb(...)sqrtu [21] | 1,245.1 ns | 40.26 ns | 203.30 ns | 1,334.5 ns |         - |
| Thompson_NFA_IsMatch     | (a|b)(...)(a|b) [27] | abcdb(...)sqrtu [21] |   776.4 ns |  4.58 ns |  23.34 ns |   769.2 ns |    3096 B |
| Thompson_DFA_IsMatch     | (a|b)(...)(a|b) [27] | abcdb(...)sqrtu [21] | 1,509.7 ns | 38.79 ns | 196.57 ns | 1,632.6 ns |         - |
| **Brzozowski_Regex_IsMatch** | **(a|b)(...)(a|b) [27]** | **bbaababa**             | **2,722.3 ns** | **28.16 ns** | **141.18 ns** | **2,678.1 ns** |    **1576 B** |
| Brzozowski_DFA_IsMatch   | (a|b)(...)(a|b) [27] | bbaababa             |   457.5 ns |  8.69 ns |  44.10 ns |   478.8 ns |         - |
| Thompson_NFA_IsMatch     | (a|b)(...)(a|b) [27] | bbaababa             | 1,942.9 ns |  9.59 ns |  49.73 ns | 1,920.5 ns |    5416 B |
| Thompson_DFA_IsMatch     | (a|b)(...)(a|b) [27] | bbaababa             |   473.2 ns |  3.68 ns |  18.22 ns |   480.2 ns |         - |
