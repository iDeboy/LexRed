using LexRed.Common;
using System.Buffers;
using System.Collections.Frozen;
using System.Runtime.InteropServices;
using System.Text;

namespace LexRed.Thompson;
public abstract class ThompsonRegex : IEquatable<ThompsonRegex>, IComparable<ThompsonRegex> {

    public static readonly ThompsonRegex EmptyString = new EmptyStringRegex();
    public static readonly ThompsonRegex EmptySet = new EmptySetRegex();
    public static readonly ThompsonRegex AnyChar = new AnyCharRegex();
    public static readonly ThompsonRegex AllChars = new AllCharsRegex();

    public abstract ThompsonRegexKind Kind { get; }

    internal protected readonly HashSet<int> _states = [];

    internal protected readonly Dictionary<int, List<(CharClass Symbols, int StateTo)>> _transitions = [];

    internal protected readonly HashSet<int> _finalStates = [];

    internal protected List<(CharClass Symbols, int StateTo)> AddOrGetResultTransition(int stateFrom) {

        ref var result = ref CollectionsMarshal.GetValueRefOrAddDefault(_transitions, stateFrom, out var exists);

        if (!exists) result = [];

        return result!;
    }

    private protected abstract int CompareToCore(ThompsonRegex other);

    public abstract CharClass[] Classy();

    internal abstract void Show(StringBuilder builder, int precedence = 0);

    public int CompareTo(ThompsonRegex? other) {

        ArgumentNullException.ThrowIfNull(other);

        if (ReferenceEquals(this, other)) return 0;

        if (Kind < other.Kind) return -1;

        if (Kind > other.Kind) return 1;

        return CompareToCore(other);

    }

    public static ThompsonRegex MakeOr(params Span<ThompsonRegex> res) {

        var flatBuffer = ArrayPool<ThompsonRegex>.Shared.Rent(res.Length * 2);
        int flatCount = 0;

        CharClassRegex? acum = null;

        foreach (var exp in res) {

            switch (exp.Kind) {
                case ThompsonRegexKind.Union:
                    var d = (UnionRegex)exp;
                    d._body.AsSpan().CopyTo(flatBuffer.AsSpan(flatCount));
                    flatCount += d._body.Length;
                    break;
                case ThompsonRegexKind.CharClass:

                    var cc = (CharClassRegex)exp;

                    if (acum is null) {
                        acum = cc;
                        continue;
                    }

                    acum = acum._charClass.Union(cc._charClass);
                    break;

                default:
                    flatBuffer[flatCount++] = exp;
                    break;
            }

        }

        if (acum is not null)
            flatBuffer[flatCount++] = acum;

        var flatSpan = flatBuffer.AsSpan(0, flatCount);

        var sortedSpan = flatSpan.Distinct();

        sortedSpan = sortedSpan.TrimStart(EmptySet);

        if (sortedSpan.Length is 0) return EmptySet;

        if (sortedSpan.Length is 1) return sortedSpan[0];

        if (sortedSpan.BinarySearch(AllChars) >= 0) return AllChars;

        var union = new UnionRegex(true, sortedSpan);

        ArrayPool<ThompsonRegex>.Shared.Return(flatBuffer);

        return union;

    }

    public static ThompsonRegex MakeConcat(params Span<ThompsonRegex> res) {

        if (res.IsEmpty) return EmptySet;

        if (res.Length is 1) return res[0];

        if (res.Length is 2) return MakeConcat(left: res[0], right: res[1]);

        return MakeConcat(left: res[0], right: MakeConcat(res[1..]));
    }

    public static ThompsonRegex MakeConcat(ThompsonRegex left, ThompsonRegex right) {

        // (r · s) · t ≈ r · (s · t)
        while (left.Kind == ThompsonRegexKind.Concatenation) {

            var leftConcat = (ConcatenationRegex)left;

            right = MakeConcat(leftConcat._right, right);
            left = leftConcat._left;
        }

        if (left.Kind is ThompsonRegexKind.EmptySet
            || right.Kind is ThompsonRegexKind.EmptySet)
            return EmptySet;

        if (left.Kind is ThompsonRegexKind.EmptyString)
            return right;

        if (right.Kind is ThompsonRegexKind.EmptyString)
            return left;

        return new ConcatenationRegex(left, right);
    }

    public static ThompsonRegex MakeKleene(ThompsonRegex re) {
        return re.Kind switch {
            ThompsonRegexKind.EmptySet => EmptyString,
            ThompsonRegexKind.EmptyString => EmptyString,
            ThompsonRegexKind.AnyChar => AllChars,
            ThompsonRegexKind.AllChars => AllChars,
            ThompsonRegexKind.Kleene => re,
            _ => new KleeneRegex(re),
        };
    }

    public NFA MakeNFA() {
        return new NFA(0, _states.ToFrozenSet(), _transitions.ToFrozenDictionary(), _finalStates.ToFrozenSet(), Classy());
    }

    public override string ToString() {

        StringBuilder sb = new(256);
        Show(sb);
        return sb.ToString();
    }

    public bool Equals(ThompsonRegex? other) {

        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return CompareTo(other) == 0;
    }
}
