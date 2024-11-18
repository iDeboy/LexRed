using System.Buffers;

namespace LexRed.Brzozowski;
public readonly struct CharClass : IComparable<CharClass> {

    private readonly bool _isPos;
    private readonly char[]? _chars;

    private const int MaxElements = char.MaxValue + 1;

    public static readonly CharClass Any = default;
    public static readonly CharClass None = Any.CreateOposite();

    public static CharClass CreatePos(string? chars) => new(true, chars);

    public static CharClass CreateNeg(string? chars) => new(false, chars);

    public readonly bool IsPositive => _isPos;

    public readonly bool IsNegative => !_isPos;

    public readonly char First {
        get {

            Span<char> span = _chars;

            if (_isPos) {

                if (span.IsEmpty) throw new InvalidOperationException("Can not extract character from empty.");

                return span[0];
            }

            if (span.Length is MaxElements) throw new InvalidOperationException("Can not extract character from entire alphabet.");

            for (int i = char.MinValue; i <= char.MaxValue; ++i) {

                char ch = (char)i;

                if (Contains(ch)) return ch;
            }

            throw new InvalidOperationException("Can not extract character from entire alphabet.");
        }
    }

    internal CharClass(bool isPos, ReadOnlySpan<char> chars) {

        _isPos = isPos;

        char[] pool = ArrayPool<char>.Shared.Rent(chars.Length);

        chars.CopyTo(pool);

        _chars = pool.AsSpan(..chars.Length).Distinct().ToArray();

        ArrayPool<char>.Shared.Return(pool);
    }

    internal CharClass(bool isPos, char[]? chars) {
        _isPos = isPos;
        _chars = chars;
    }

    public CharClass CreateOposite() => new(!_isPos, _chars);

    public readonly bool Contains(char c) => _isPos ? _chars.AsSpan().Contains(c) : !_chars.AsSpan().Contains(c);

    public CharClass Intersection(CharClass other) {

        ReadOnlySpan<char> chars = _chars;
        ReadOnlySpan<char> ohterChars = other._chars;

        return (_isPos, other._isPos) switch {
            (true, true) => new(true, chars.Intersect(ohterChars)),
            (true, false) => new(true, chars.Except(ohterChars)),
            (false, true) => new(true, ohterChars.Except(chars)),
            (false, false) => new(false, chars.Union(ohterChars)),
        };

    }

    public CharClass Union(CharClass other) {

        ReadOnlySpan<char> chars = _chars;
        ReadOnlySpan<char> ohterChars = other._chars;

        return (_isPos, other._isPos) switch {
            (true, true) => new(true, chars.Union(ohterChars)),
            (true, false) => new(false, ohterChars.Except(chars)),
            (false, true) => new(false, chars.Except(ohterChars)),
            (false, false) => new(false, chars.Intersect(ohterChars)),
        };
    }

    public int CompareTo(CharClass other) {

        var compare = _isPos.CompareTo(other._isPos);

        if (compare != 0) return -compare;

        return _chars.AsSpan().SequenceCompareTo(other._chars);
    }
}
