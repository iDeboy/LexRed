﻿using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
    .Run(args);

// BenchmarkRunner.Run<AutomatonCreationBenchmark>();
