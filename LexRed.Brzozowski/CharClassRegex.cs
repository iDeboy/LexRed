using LexRed.Common;
using System.Diagnostics;
using System.Text;

namespace LexRed.Brzozowski;
public class CharClassRegex : BrzozowskiRegex {
    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.CharClass;

    public override bool IsNullable => false;

    internal readonly CharClass _charClass;

    internal CharClassRegex(CharClass charClass) => _charClass = charClass;

    public override CharClass[] Classy() {
        return [_charClass.CreateOposite(), _charClass];
    }

    public override BrzozowskiRegex Derive(char ch) {

        if (_charClass.Contains(ch)) return EmptyString;

        return EmptySet;
    }

    internal override void Show(StringBuilder builder, int precedence = 0) {
        builder.Append(_charClass.ToString());
    }

    private protected override int CompareToCore(BrzozowskiRegex other) {

        Debug.Assert(other is CharClassRegex);

        var charClassRegex = (CharClassRegex)other;

        return _charClass.CompareTo(charClassRegex._charClass);
    }

    public static implicit operator CharClassRegex(CharClass charClass) {
        return new CharClassRegex(charClass);
    }
}

internal sealed class EmptySetRegex : CharClassRegex {

    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.EmptySet;

    public EmptySetRegex() : base(CharClass.None) { }

    private protected override int CompareToCore(BrzozowskiRegex other)
        => 0;

}

internal sealed class AnyCharRegex : CharClassRegex {

    public override BrzozowskiRegexKind Kind => BrzozowskiRegexKind.AnyChar;

    public AnyCharRegex() : base(CharClass.Any) { }

    private protected override int CompareToCore(BrzozowskiRegex other)
        => 0;
}
