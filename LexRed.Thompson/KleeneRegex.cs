using LexRed.Common;
using System.Diagnostics;
using System.Text;

namespace LexRed.Thompson;
public class KleeneRegex : ThompsonRegex {
    public override ThompsonRegexKind Kind => ThompsonRegexKind.Kleene;

    internal readonly ThompsonRegex _inner;

    internal KleeneRegex(ThompsonRegex re) {

        _inner = re;

        _states.Add(0);

        const int start = 1;

        foreach (var state in re._states)
            _states.Add(start + state);

        var startResult = AddOrGetResultTransition(0);
        startResult.Add((CharClass.Epsilon, 1));
        int end = _states.Count;
        _states.Add(end);
        startResult.Add((CharClass.Epsilon, end));

        foreach (var (stateFrom, resultRe) in re._transitions) {

            var result = AddOrGetResultTransition(start + stateFrom);

            foreach (var (symbols, stateTo) in resultRe)
                result.Add((symbols, start + stateTo));

        }

        foreach (var final in re._finalStates) {

            var result = AddOrGetResultTransition(start + final);
            result.Add((CharClass.Epsilon, 1));
            result.Add((CharClass.Epsilon, end));

        }

        _finalStates.Add(end);
    }

    private protected override int CompareToCore(ThompsonRegex other) {

        Debug.Assert(other is KleeneRegex);

        var kleene = (KleeneRegex)other;

        return _inner.CompareTo(kleene._inner);

    }

    internal override void Show(StringBuilder builder, int precedence = 0) {

        _inner.Show(builder, 4);

        builder.Append('*');
    }

    public override CharClass[] Classy()
        => _inner.Classy();
}

internal sealed class AllCharsRegex : KleeneRegex {

    public override ThompsonRegexKind Kind => ThompsonRegexKind.AllChars;

    internal AllCharsRegex()
        : base(AnyChar) { }

}
