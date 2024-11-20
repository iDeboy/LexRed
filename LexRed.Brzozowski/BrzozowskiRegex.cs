using System.Buffers;
using System.Text;

namespace LexRed.Brzozowski;

using ExploreReturn = (HashSet<int> States, Dictionary<(int State, char Symbol), int> Transitions, HashSet<int> FinalStates);
using GotoReturn = (SortedSet<BrzozowskiRegex> States, SortedDictionary<(BrzozowskiRegex State, CharClass Symbols), BrzozowskiRegex> Transitions);
using RegexMap = SortedDictionary<BrzozowskiRegex, int>;

// https://crypto.stanford.edu/~blynn/haskell/re.html
public abstract class BrzozowskiRegex : IEquatable<BrzozowskiRegex>, IComparable<BrzozowskiRegex> {

    public static readonly BrzozowskiRegex EmptySet = new EmptySetRegex();
    public static readonly BrzozowskiRegex AnyChar = new AnyCharRegex();
    public static readonly BrzozowskiRegex EmptyString = new EmptyStringRegex();
    public static readonly BrzozowskiRegex AllChars = new AllCharsRegex();

    public abstract BrzozowskiRegexKind Kind { get; }

    public abstract bool IsNullable { get; }

    public abstract CharClass[] Classy();

    public abstract BrzozowskiRegex Derive(char ch);

    internal abstract void Show(StringBuilder builder, int precedence = 0);

    private protected abstract int CompareToCore(BrzozowskiRegex other);

    public bool IsMatch(ReadOnlySpan<char> input) {

        return input switch {

        [] => IsNullable,
        [var ch, .. var rest] => Derive(ch).IsMatch(rest),
        };

    }

    public int CompareTo(BrzozowskiRegex? other) {

        ArgumentNullException.ThrowIfNull(other);

        if (ReferenceEquals(this, other)) return 0;

        if (Kind < other.Kind) return -1;

        if (Kind > other.Kind) return 1;

        return CompareToCore(other);
    }
    public bool Equals(BrzozowskiRegex? other) {

        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return CompareTo(other) == 0;
    }

    public static BrzozowskiRegex MakeOr(params Span<BrzozowskiRegex> res) {

        var flatBuffer = ArrayPool<BrzozowskiRegex>.Shared.Rent(res.Length * 2);
        int flatCount = 0;

        CharClassRegex? acum = null;

        foreach (var exp in res) {

            switch (exp.Kind) {
                case BrzozowskiRegexKind.Disjunction:
                    var d = (DisjunctionRegex)exp;
                    d._body.AsSpan().CopyTo(flatBuffer.AsSpan(flatCount));
                    flatCount += d._body.Length;
                    break;
                case BrzozowskiRegexKind.CharClass:

                    var cc = (CharClassRegex)exp;

                    if (acum is null) {
                        acum = cc;
                        continue;
                    }

                    acum = acum.CharClass.Union(cc.CharClass);
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

        sortedSpan.TrimStart(EmptySet);

        if (sortedSpan.Length is 0) return EmptySet;

        if (sortedSpan.Length is 1) return sortedSpan[0];

        if (sortedSpan.BinarySearch(AllChars) >= 0) return AllChars;

        var disj = new DisjunctionRegex(true, sortedSpan);

        ArrayPool<BrzozowskiRegex>.Shared.Return(flatBuffer);

        return disj;
    }

    public static BrzozowskiRegex MakeAnd(params Span<BrzozowskiRegex> res) {

        var flatBuffer = ArrayPool<BrzozowskiRegex>.Shared.Rent(res.Length * 2);
        int flatCount = 0;

        CharClassRegex? acum = null;

        foreach (var exp in res) {

            switch (exp.Kind) {
                case BrzozowskiRegexKind.Conjunction:
                    var c = (ConjunctionRegex)exp;
                    c._body.AsSpan().CopyTo(flatBuffer.AsSpan(flatCount));
                    flatCount += c._body.Length;
                    break;
                case BrzozowskiRegexKind.CharClass:

                    var cc = (CharClassRegex)exp;

                    if (acum is null) {
                        acum = cc;
                        continue;
                    }

                    acum = acum.CharClass.Intersection(cc.CharClass);
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

        sortedSpan.TrimEnd(AllChars);

        if (sortedSpan.Length is 0) return AllChars;

        if (sortedSpan.Length is 1) return sortedSpan[0];

        if (sortedSpan.BinarySearch(EmptySet) >= 0) return EmptySet;

        var conj = new ConjunctionRegex(true, sortedSpan);

        ArrayPool<BrzozowskiRegex>.Shared.Return(flatBuffer);

        return conj;
    }

    public static BrzozowskiRegex MakeNot(BrzozowskiRegex re) {
        // Maybe add a case for the rest of constant expresions
        return re.Kind switch {
            BrzozowskiRegexKind.EmptySet => AllChars,
            BrzozowskiRegexKind.AllChars => EmptySet,
            BrzozowskiRegexKind.Complement => ((ComplementRegex)re)._inner,
            _ => new ComplementRegex(re),
        };
    }

    public static BrzozowskiRegex MakeConcat(BrzozowskiRegex left, BrzozowskiRegex right) {

        // (r · s) · t ≈ r · (s · t)
        while (left.Kind == BrzozowskiRegexKind.Concatenation) {

            var leftConcat = (ConcatenationRegex)left;

            right = MakeConcat(leftConcat._right, right);
            left = leftConcat._left;
        }

        if (left.Kind is BrzozowskiRegexKind.EmptySet
            || right.Kind is BrzozowskiRegexKind.EmptySet)
            return EmptySet;

        if (left.Kind is BrzozowskiRegexKind.EmptyString)
            return right;

        if (right.Kind is BrzozowskiRegexKind.EmptyString)
            return left;

        return new ConcatenationRegex(left, right);
    }

    public static BrzozowskiRegex MakeKleene(BrzozowskiRegex re) {
        return re.Kind switch {
            BrzozowskiRegexKind.EmptySet => EmptyString,
            BrzozowskiRegexKind.EmptyString => EmptyString,
            BrzozowskiRegexKind.AnyChar => AllChars,
            BrzozowskiRegexKind.AllChars => AllChars,
            BrzozowskiRegexKind.Kleene => re,
            _ => new KleeneRegex(re),
        };
    }

    public DFA MakeDFA() {

        var (states, transitions, finalStates) = Explore([this], [], this);

        return new(0, states, transitions, finalStates);
    }

    private ExploreReturn Explore(SortedSet<BrzozowskiRegex> states, SortedDictionary<(BrzozowskiRegex State, CharClass Symbols), BrzozowskiRegex> transitions, BrzozowskiRegex state) {

        var newStates = states;
        var newTransitions = transitions;

        foreach (var symbols in state.Classy()) {
            (newStates, newTransitions) = Goto(state, symbols, newStates, newTransitions);
        }

        return default;
    }

    private GotoReturn Goto(BrzozowskiRegex state, CharClass symbols, SortedSet<BrzozowskiRegex> states, SortedDictionary<(BrzozowskiRegex State, CharClass Symbols), BrzozowskiRegex> transitions) {

        char c = symbols.First;
        var qc = state.Derive(c);

        if (states.TryGetValue(state, out var found)) {

        }

        return default;
    }

    private int LookupOrInsert() {
        return 0;
    }

    public override string ToString() {

        StringBuilder sb = new(256);
        Show(sb);
        return sb.ToString();

    }

}
