using System.Buffers;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace LexRed.Brzozowski;
public class ConjunctionRegex : BrzozowskiRegex {
    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.Conjunction;

    public override bool IsNullable { get; }

    internal readonly BrzozowskiRegex[] _body;

    internal ConjunctionRegex(bool sorted, params Span<BrzozowskiRegex> body) {

        Debug.Assert(!body.IsEmpty);

        var distinct = sorted ? body : body.Distinct();
        IsNullable = distinct[0].IsNullable;

        for (int i = 1; i < distinct.Length && IsNullable; ++i) {
            IsNullable = IsNullable && distinct[i].IsNullable;
        }

        _body = distinct.ToArray();
    }

    public override CharClass[] Classy() {

        Debug.Assert(_body.Length > 0);

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

        var and = MakeAnd(span);

        ArrayPool<BrzozowskiRegex>.Shared.Return(pool, true);

        return and;

    }

    private protected override int CompareToCore(BrzozowskiRegex other) {

        if (other is not ConjunctionRegex conjunctionRegex) throw new ArgumentException("Compared regex is not ConjunctionRegex");

        if (_body.Length > conjunctionRegex._body.Length) return -1;

        if (_body.Length < conjunctionRegex._body.Length) return 1;

        int length = _body.Length;

        for (int i = 0; i < length; ++i) {

            var compare = _body[i].CompareTo(conjunctionRegex._body[i]);

            if (compare is not 0) return compare;

        }

        return 0;

    }

    internal override void Show(StringBuilder builder, int precedence = 0) {

        ReadOnlySpan<BrzozowskiRegex> bodySpan = _body;

        if (bodySpan.IsEmpty) return;

        builder.AppendIf(precedence > 1, '(');

        bodySpan[0].Show(builder, 1);

        for (int i = 1; i < bodySpan.Length; ++i) {
            builder.Append('&');
            bodySpan[i].Show(builder, 1);
        }

        builder.AppendIf(precedence > 1, ')');

    }
}
