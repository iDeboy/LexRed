using LexRed.Common;
using System.Diagnostics;
using System.Text;

namespace LexRed.Thompson;
public class ConcatenationRegex : ThompsonRegex {
    public override ThompsonRegexKind Kind => ThompsonRegexKind.Concatenation;

    internal readonly ThompsonRegex _left;
    internal readonly ThompsonRegex _right;

    internal ConcatenationRegex(ThompsonRegex left, ThompsonRegex right) {

        (_left, _right) = (left, right);

        var middle = left._states.Count;

        foreach (var state in left._states)
            _states.Add(state);

        foreach (var (stateFrom, result) in left._transitions)
            _transitions.Add(stateFrom, result);

        foreach (var state in left._finalStates) {
            var result = AddOrGetResultTransition(state);
            result.Add((CharClass.Epsilon, middle));
        }

        foreach (var state in right._states)
            _states.Add(middle + state);

        foreach (var (stateFrom, resultRight) in right._transitions) {

            var result = AddOrGetResultTransition(middle + stateFrom);

            foreach (var (symbols, stateTo) in resultRight)
                result.Add((symbols, middle + stateTo));

        }

        foreach (var final in right._finalStates) {
            _finalStates.Add(middle + final);
        }

    }

    private protected override int CompareToCore(ThompsonRegex other) {

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

    public override CharClass[] Classy() {

        ReadOnlySpan<CharClass> result = _left.Classy();

        return result.AllPairs(_right.Classy());
    }
}
