using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;

namespace LexRed.Brzozowski;
public class KleeneRegex : BrzozowskiRegex {
    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.Kleene;

    public override bool IsNullable => true;

    internal readonly BrzozowskiRegex _inner;

    internal KleeneRegex(BrzozowskiRegex inner)
        => _inner = inner;

    public override CharClass[] Classy()
        => _inner.Classy();

    public override BrzozowskiRegex Derive(char ch) 
        => MakeConcat(_inner.Derive(ch), this);

    private protected override int CompareToCore(BrzozowskiRegex other) {

        Debug.Assert(other is KleeneRegex);

        var kleene = (KleeneRegex)other;

        return _inner.CompareTo(kleene._inner);
    }

    internal override void Show(StringBuilder builder, int precedence = 0) {

        _inner.Show(builder, 4);

        builder.Append('*');

    }
}

internal sealed class EmptyStringRegex : KleeneRegex {

    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.EmptyString;

    internal EmptyStringRegex()
        : base(EmptySet) { }

    internal override void Show(StringBuilder builder, int precedence = 0) {
        builder.Append('ε');
    }
}

internal sealed class AllCharsRegex : KleeneRegex {

    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.AllChars;

    internal AllCharsRegex()
        : base(AnyChar) { }

}

