using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace LexRed.Brzozowski;
public readonly struct CharClass : IComparable<CharClass> {

    private readonly bool _isPos;
    private readonly char[]? _chars;

    private const int MaxElements = char.MaxValue + 1;

    public static readonly CharClass Any = default;
    public static readonly CharClass None = Any.CreateOposite();

    public static CharClass CreatePos(ReadOnlySpan<char> chars) => new(true, chars);

    public static CharClass CreateNeg(ReadOnlySpan<char> chars) => new(false, chars);

    public readonly bool IsPositive => _isPos;

    public readonly bool IsNegative => !_isPos;

    public readonly bool IsNone => _isPos && _chars.AsSpan().IsEmpty;

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


    private static void AddToResult(StringBuilder builder, char low, char high) {

        if (low == high)
            builder.Append(low);
        else if (low.GetNextChar() == high) {
            builder.Append(low);
            builder.Append(high);
        }
        else if (low.GetNextChar(2) == high) {
            builder.Append(low);
            builder.Append(low.GetNextChar());
            builder.Append(high);
        }
        else {
            builder.Append(low);
            builder.Append('-');
            builder.Append(high);
        }

    }

    private void RangeFinder(StringBuilder builder) {

        ReadOnlySpan<char> span = _chars.AsSpan();

        if (span.IsEmpty) return;

        char low = span[0];
        char high = low;

        if (span.Length is 1) {
            builder.Append(low);
            return;
        }

        var rest = span[1..];

        foreach (char current in rest) {

            if (current == high.GetNextChar()) {
                high = current;
                continue;
            }

            AddToResult(builder, low, high);

            low = high = current;
        }

        AddToResult(builder, low, high);

        return;
    }

    public readonly override string ToString() {

        StringBuilder sb = new(256);

        var span = _chars.AsSpan();

        switch ((_isPos, span.Length)) {
            case (true, 0):
                sb.Append('[');
                sb.Append(']');
                break;
            case (true, 1):
                sb.Append(span);
                break;
            case (false, 0):
                sb.Append('.');
                break;
            default:
                sb.Append('[');
                sb.AppendIf(IsNegative, '^');
                RangeFinder(sb);
                sb.Append(']');
                break;
        }

        return sb.ToString();

    }

    public readonly Enumerator GetEnumerator() => new(ref Unsafe.AsRef(in this));

    public ref struct Enumerator {

        private char _current;
        private readonly ReadOnlySpan<char> _chars;
        private readonly bool _isPos;

        private int _index;

        internal Enumerator(ref readonly CharClass charClass) {

            _chars = charClass._chars;
            _index = -1;

        }

        public readonly char Current => _current;

        public bool MoveNext() {

            int index = _index + 1;

            if (_isPos) {

                if (index < _chars.Length) {
                    _index = index;
                    return true;
                }

                return false;

            }
            else {

                if (index > char.MaxValue) return false;

                char ch = (char)index;

                while (_chars.Contains(ch)) {

                    ch = (char)++index;

                    if (index > char.MaxValue) return false;
                }

                _current = ch;
                _index = index;

                return true;
            }

        }
    }

}
