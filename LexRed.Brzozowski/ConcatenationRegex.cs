using System.Diagnostics;
using System.Text;

namespace LexRed.Brzozowski;
public class ConcatenationRegex : BrzozowskiRegex {
    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.Concatenation;

    public override bool IsNullable => _left.IsNullable && _right.IsNullable;

    internal readonly BrzozowskiRegex _left;
    internal readonly BrzozowskiRegex _right;

    internal ConcatenationRegex(BrzozowskiRegex left, BrzozowskiRegex right)
        => (_left, _right) = (left, right);

    public override CharClass[] Classy() {

        if (_left.IsNullable) {

            ReadOnlySpan<CharClass> result = _left.Classy();

            return result.AllPairs(_right.Classy());
        }

        return _left.Classy();
    }

    public override BrzozowskiRegex Derive(char ch) {

        var concat = MakeConcat(_left.Derive(ch), _right);

        if (_left.IsNullable) return MakeOr(concat, _right.Derive(ch));

        return concat;

    }

    private protected override int CompareToCore(BrzozowskiRegex other) {

        Debug.Assert(other is ConcatenationRegex);

        var concat = (ConcatenationRegex)other;

        return (_left, _right).CompareTo((concat._left, concat._right));
    }

    internal override void Show(StringBuilder builder, int precedence = 0) {

        builder.AppendIf(precedence > 2, '(');

        _left.Show(builder, 2);
        _right.Show(builder, 2);

        builder.AppendIf(precedence > 2, ')');

    }
}
