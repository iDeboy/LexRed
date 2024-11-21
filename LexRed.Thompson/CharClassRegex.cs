using LexRed.Common;
using System.Diagnostics;
using System.Text;

namespace LexRed.Thompson;
public class CharClassRegex : ThompsonRegex {
    public override ThompsonRegexKind Kind => ThompsonRegexKind.CharClass;

    internal readonly CharClass _charClass;

    public CharClassRegex(CharClass charClass) {

        _charClass = charClass;

        _states.Add(0);
        _states.Add(1);

        _transitions.Add(0, [(charClass, 1)]);

        if (!charClass.IsNone) {
            _finalStates.Add(1);
        }

    }

    public static implicit operator CharClassRegex(CharClass charClass) {
        return new CharClassRegex(charClass);
    }

    private protected override int CompareToCore(ThompsonRegex other) {

        Debug.Assert(other is CharClassRegex);

        var charClassRegex = (CharClassRegex)other;

        return _charClass.CompareTo(charClassRegex._charClass);
    }

    internal override void Show(StringBuilder builder, int precedence = 0) {
        builder.Append(_charClass.ToString());
    }

    public override CharClass[] Classy()
        => [_charClass.CreateOposite(), _charClass];
}

internal sealed class EmptyStringRegex : CharClassRegex {

    public override ThompsonRegexKind Kind => ThompsonRegexKind.EmptyString;

    public EmptyStringRegex() : base(CharClass.Epsilon) { }

    internal override void Show(StringBuilder builder, int precedence = 0) {
        builder.Append('ε');
    }

}
internal sealed class EmptySetRegex : CharClassRegex {

    public override ThompsonRegexKind Kind => ThompsonRegexKind.EmptySet;

    public EmptySetRegex() : base(CharClass.None) { }

    private protected override int CompareToCore(ThompsonRegex other)
        => 0;

}

internal sealed class AnyCharRegex : CharClassRegex {

    public override ThompsonRegexKind Kind => ThompsonRegexKind.AnyChar;

    public AnyCharRegex() : base(CharClass.Any) { }

    private protected override int CompareToCore(ThompsonRegex other)
        => 0;

}