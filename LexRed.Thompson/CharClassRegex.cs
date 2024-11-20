using LexRed.Common;

namespace LexRed.Thompson;
public class CharClassRegex : ThompsonRegex {
    public override ThompsonRegexKind Kind => ThompsonRegexKind.CharClass;

    public CharClassRegex(CharClass charClass) {

        const int start = 0;
        const int end = 1;

        _transitions.Add(start, [(charClass, end)]);
        _finalStates.Add(end);

    }

    public static implicit operator CharClassRegex(CharClass charClass) {
        return new CharClassRegex(charClass);
    }

    //private protected override int CompareToCore(ThompsonRegex other) {

    //    Debug.Assert(other is CharClassRegex);

    //    var cc = (CharClassRegex)other;

    //    return CharClass.CompareTo(cc.CharClass);
    //}
}
