using LexRed.Common;

namespace LexRed.Thompson;
public abstract class ThompsonRegex /*: IComparable<ThompsonRegex>*/ {

    public abstract ThompsonRegexKind Kind { get; }

    internal protected readonly SortedDictionary<int, List<(CharClass Symbols, int StateTo)>> _transitions = [];

    internal protected readonly HashSet<int> _finalStates = [];

    //private protected abstract int CompareToCore(ThompsonRegex other);

    //public int CompareTo(ThompsonRegex? other) {

    //    ArgumentNullException.ThrowIfNull(other);

    //    if (ReferenceEquals(this, other)) return 0;

    //    if (Kind < other.Kind) return -1;

    //    if (Kind > other.Kind) return 1;

    //    return CompareToCore(other);

    //}


}
