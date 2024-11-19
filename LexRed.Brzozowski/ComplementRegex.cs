using System.Linq.Expressions;
using System.Text;

namespace LexRed.Brzozowski;
public class ComplementRegex : BrzozowskiRegex {
    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.Complement;

    public override bool IsNullable => !_inner.IsNullable;

    internal readonly BrzozowskiRegex _inner;

    internal ComplementRegex(BrzozowskiRegex inner) 
        => _inner = inner;

    public override CharClass[] Classy() 
        => _inner.Classy();

    public override BrzozowskiRegex Derive(char ch)
        => MakeNot(_inner.Derive(ch));

    private protected override int CompareToCore(BrzozowskiRegex other) {

        if (other is not ComplementRegex complementRegex) throw new ArgumentException($"Compared regex is not ComplementRegex");

        return _inner.CompareTo(complementRegex._inner);
    }

    internal override void Show(StringBuilder builder, int precedence = 0) {

        builder.Append('~');

        builder.AppendIf(precedence > 3, '(');

        _inner.Show(builder, 3);

        builder.AppendIf(precedence > 3, ')');

    }
}
