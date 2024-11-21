using LexRed.Common;
using System.Buffers;
using System.Diagnostics;
using System.Text;

namespace LexRed.Brzozowski;
public class DisjunctionRegex : BrzozowskiRegex {
    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.Disjunction;

    public override bool IsNullable { get; }

    internal readonly BrzozowskiRegex[] _body;

    internal DisjunctionRegex(bool sorted, params Span<BrzozowskiRegex> body) {

        Debug.Assert(!body.IsEmpty);

        var distinct = sorted ? body : body.Distinct();
        IsNullable = distinct[0].IsNullable;

        for (int i = 1; i < distinct.Length && !IsNullable; ++i) {

            IsNullable = IsNullable || distinct[i].IsNullable;
        }

        _body = distinct.ToArray();

    }

    public override CharClass[] Classy() {

        Debug.Assert(_body.Length > 1);

        ReadOnlySpan<CharClass> result = _body[0].Classy();

        for (int i = 1; i < _body.Length; ++i) {
            result = result.AllPairs(_body[i].Classy());
        }

        return result.ToArray();
    }

    public override BrzozowskiRegex Derive(char ch) {

        var pool = ArrayPool<BrzozowskiRegex>.Shared.Rent(_body.Length);

        var body = _body.AsSpan();

        Span<BrzozowskiRegex> span = pool.AsSpan(0, _body.Length);

        for (int i = 0; i < body.Length; i++) {

            var exp = body[i];

            span[i] = exp.Derive(ch);

        }

        var or = MakeOr(span);

        ArrayPool<BrzozowskiRegex>.Shared.Return(pool, true);

        return or;
    }

    private protected override int CompareToCore(BrzozowskiRegex other) {

        if (other is not DisjunctionRegex disjuctionRegex) throw new ArgumentException("Compared regex is not DisjunctionRegex");

        if (_body.Length > disjuctionRegex._body.Length) return -1;

        if (_body.Length < disjuctionRegex._body.Length) return 1;

        int length = _body.Length;

        for (int i = 0; i < length; ++i) {

            var compare = _body[i].CompareTo(disjuctionRegex._body[i]);

            if (compare is not 0) return compare;

        }

        return 0;
    }

    internal override void Show(StringBuilder builder, int precedence = 0) {

        ReadOnlySpan<BrzozowskiRegex> bodySpan = _body;

        if (bodySpan.IsEmpty) return;

        builder.AppendIf(precedence > 0, '(');

        bodySpan[0].Show(builder, 0);

        for (int i = 1; i < bodySpan.Length; ++i) {
            builder.Append('|');
            bodySpan[i].Show(builder, 0);
        }

        builder.AppendIf(precedence > 0, ')');
    }
}
