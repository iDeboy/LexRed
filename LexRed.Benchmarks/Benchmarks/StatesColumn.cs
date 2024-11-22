using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace LexRed.Benchmarks.Benchmarks;
public sealed class StatesColumn : IColumn {
    public string Id => throw new NotImplementedException();

    public string ColumnName => throw new NotImplementedException();

    public bool AlwaysShow => throw new NotImplementedException();

    public ColumnCategory Category => throw new NotImplementedException();

    public int PriorityInCategory => throw new NotImplementedException();

    public bool IsNumeric => throw new NotImplementedException();

    public UnitType UnitType => throw new NotImplementedException();

    public string Legend => throw new NotImplementedException();

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase) {
        var m = benchmarkCase.Descriptor.WorkloadMethod;
        return "";
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) {
        throw new NotImplementedException();
    }

    public bool IsAvailable(Summary summary) {
        throw new NotImplementedException();
    }

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) {
        throw new NotImplementedException();
    }
}
