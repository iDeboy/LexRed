Esta fase del proyecto plantea comparar los siguientes algoritmos/métodos:
 - Algoritmo de Thompson
 - Método de Brzozowski

# Algoritmo de Thompson
El algoritmo de Thompson convierte una expresion regular (RE) a su autómata finito no determinista (NDFA) equivalente y luego hay que aplicar el *Algoritmo de subconjuntos* 
para convertir ese NDFA a su autómata finito determinista (DFA) equivalente.

# Método de Thompson
Este método convierte una expresión regular (RE) a su autómata finito determinista (DFA) equivalente sin tener que hacer otra conversión.

# Resultados
El proyecto plantea mostrar un benchmark de ambos algoritmos:
 - Benchmark de contrucción de los automátas
 - Benchmark de la evalucación de cadenas

## Benchmark de contrucción
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

## Benchmark de evaluación
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
| **Brzozowski_Regex_IsMatch** | **((a\|b(...)\*t)\*u [56]** | **abcdb(...)sqrtu [21]** | **4,978.7 ns** | **21.58 ns** | **110.55 ns** | **4,925.5 ns** |    **5552 B** |
| Brzozowski_DFA_IsMatch   | ((a\|b(...)\*t)\*u [56] | abcdb(...)sqrtu [21] |   742.7 ns |  9.02 ns |  44.19 ns |   755.1 ns |         - |
| Thompson_NFA_IsMatch     | ((a\|b(...)\*t)\*u [56] | abcdb(...)sqrtu [21] | 4,251.6 ns | 31.74 ns | 154.96 ns | 4,267.7 ns |   10416 B |
| Thompson_DFA_IsMatch     | ((a\|b(...)\*t)\*u [56] | abcdb(...)sqrtu [21] | 2,064.7 ns | 29.78 ns | 150.93 ns | 2,111.9 ns |         - |
| **Brzozowski_Regex_IsMatch** | **((a\|b(...)\*t)\*u [56]** | **bbaababa**             | **1,619.3 ns** | **10.14 ns** |  **51.87 ns** | **1,601.5 ns** |    **1936 B** |
| Brzozowski_DFA_IsMatch   | ((a\|b(...)\*t)\*u [56] | bbaababa             |   308.4 ns |  7.26 ns |  36.53 ns |   320.8 ns |         - |
| Thompson_NFA_IsMatch     | ((a\|b(...)\*t)\*u [56] | bbaababa             | 1,508.6 ns |  8.99 ns |  46.77 ns | 1,490.2 ns |    4296 B |
| Thompson_DFA_IsMatch     | ((a\|b(...)\*t)\*u [56] | bbaababa             |   964.2 ns | 28.31 ns | 145.54 ns |   943.9 ns |         - |
| **Brzozowski_Regex_IsMatch** | **(a\|b)(...)(a\|b) [27]** | **abcdb(...)sqrtu [21]** |   **864.9 ns** |  **5.56 ns** |  **28.74 ns** |   **876.0 ns** |     **664 B** |
| Brzozowski_DFA_IsMatch   | (a\|b)(...)(a\|b) [27] | abcdb(...)sqrtu [21] | 1,245.1 ns | 40.26 ns | 203.30 ns | 1,334.5 ns |         - |
| Thompson_NFA_IsMatch     | (a\|b)(...)(a\|b) [27] | abcdb(...)sqrtu [21] |   776.4 ns |  4.58 ns |  23.34 ns |   769.2 ns |    3096 B |
| Thompson_DFA_IsMatch     | (a\|b)(...)(a\|b) [27] | abcdb(...)sqrtu [21] | 1,509.7 ns | 38.79 ns | 196.57 ns | 1,632.6 ns |         - |
| **Brzozowski_Regex_IsMatch** | **(a\|b)(...)(a\|b) [27]** | **bbaababa**             | **2,722.3 ns** | **28.16 ns** | **141.18 ns** | **2,678.1 ns** |    **1576 B** |
| Brzozowski_DFA_IsMatch   | (a\|b)(...)(a\|b) [27] | bbaababa             |   457.5 ns |  8.69 ns |  44.10 ns |   478.8 ns |         - |
| Thompson_NFA_IsMatch     | (a\|b)(...)(a\|b) [27] | bbaababa             | 1,942.9 ns |  9.59 ns |  49.73 ns | 1,920.5 ns |    5416 B |
| Thompson_DFA_IsMatch     | (a\|b)(...)(a\|b) [27] | bbaababa             |   473.2 ns |  3.68 ns |  18.22 ns |   480.2 ns |         - |
