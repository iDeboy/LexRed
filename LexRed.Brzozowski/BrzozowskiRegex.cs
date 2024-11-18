namespace LexRed.Brzozowski;
// https://crypto.stanford.edu/~blynn/haskell/re.html
public abstract class BrzozowskiRegex : IComparable<BrzozowskiRegex> {

    public static readonly BrzozowskiRegex EmptySet;
    public static readonly BrzozowskiRegex AnyChar;
    public static readonly BrzozowskiRegex EmptyString;
    public static readonly BrzozowskiRegex AllChars;

    public abstract BrzozowskiRegexKind Kind { get; }

    public abstract bool IsNullable { get; }

    public abstract CharClass[] Classy();

    public abstract BrzozowskiRegex Derive(char ch);

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
}
