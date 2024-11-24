using LexRed.Common;
using System.Buffers;
using System.Collections.Frozen;
using System.Text;

namespace LexRed.Brzozowski;

// using ExploreReturn = (HashSet<BrzozowskiRegex> States, Dictionary<(BrzozowskiRegex State, CharClass Symbols), BrzozowskiRegex> Transitions, HashSet<BrzozowskiRegex> FinalStates);
using ExploreReturn = (SortedDictionary<BrzozowskiRegex, int> States, Dictionary<(int State, CharClass Symbols), int> Transitions, HashSet<int> FinalStates);

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

                    acum = acum._charClass.Intersection(cc._charClass);
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

        sortedSpan = sortedSpan.TrimEnd(AllChars);

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

    public static BrzozowskiRegex MakeConcat(params Span<BrzozowskiRegex> res) {

        if (res.IsEmpty) return EmptySet;

        if (res.Length is 1) return res[0];

        if (res.Length is 2) return MakeConcat(left: res[0], right: res[1]);

        return MakeConcat(left: res[0], right: MakeConcat(res[1..]));
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

        // TODO: get rid of recursion
        var (statesRe, transitionsRe, finalStatesRe) = Explore(new() { [this] = 0 }, [], (this, 0), IsNullable ? [0] : []);

        FrozenSet<int> states = statesRe.Values.ToFrozenSet();
        FrozenSet<int> finalStates = finalStatesRe.ToFrozenSet();
        FrozenDictionary<(int State, CharClass Symbols), int> transitions = transitionsRe.ToFrozenDictionary();

        

        return new DFA(0, states, transitions, finalStates);
    }

    private ExploreReturn Explore(SortedDictionary<BrzozowskiRegex, int> states, Dictionary<(int State, CharClass Symbols), int> transitions, (BrzozowskiRegex Re, int Index) state, HashSet<int> finalStates) {

        var newStates = states;
        var newTransitions = transitions;
        var newFinalStates = finalStates;

        foreach (var symbols in state.Re.Classy()) {

            if (symbols.IsNone) continue;

            (newStates, newTransitions, newFinalStates) = Goto(state, symbols, newStates, newTransitions, newFinalStates);
        }

        return (newStates, newTransitions, newFinalStates);
    }

    private ExploreReturn Goto((BrzozowskiRegex Re, int Index) state, CharClass symbols, SortedDictionary<BrzozowskiRegex, int> states, Dictionary<(int State, CharClass Symbols), int> transitions, HashSet<int> finalStates) {

        char c = symbols.First;
        var qc = state.Re.Derive(c);

        // if exists any q in Q such that q is equivalent to qc
        if (states.TryGetValue(qc, out var q)) {
            transitions.Add((state.Index, symbols), q);
            return (states, transitions, finalStates);
        }

        var index = states.Count;
        states.Add(qc, index);

        if (qc.IsNullable) finalStates.Add(index);

        transitions.Add((state.Index, symbols), index);

        return Explore(states, transitions, (qc, index), finalStates);
    }

    public override string ToString() {

        StringBuilder sb = new(256);
        Show(sb);
        return sb.ToString();

    }

}
