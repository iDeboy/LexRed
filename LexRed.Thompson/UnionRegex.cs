using LexRed.Common;
using System.Diagnostics;
using System.Text;

namespace LexRed.Thompson;
public class UnionRegex : ThompsonRegex {

    public override ThompsonRegexKind Kind => ThompsonRegexKind.Union;

    internal readonly ThompsonRegex[] _body;

    internal UnionRegex(bool sorted, params Span<ThompsonRegex> body) {

        Debug.Assert(body.Length > 1);

        var distinct = sorted ? body : body.Distinct();

        int start = 1;

        List<int> finals = [];

        _states.Add(0);

        foreach (var re in distinct) {

            foreach (var state in re._states)
                _states.Add(start + state);

            AddOrGetResultTransition(0)
                .Add((CharClass.Epsilon, start));

            foreach (var (stateFrom, resultRe) in re._transitions) {

                var result = AddOrGetResultTransition(start + stateFrom);

                foreach (var (symbols, stateTo) in resultRe) {
                    result.Add((symbols, start + stateTo));
                }

            }

            foreach (var final in re._finalStates)
                finals.Add(start + final);

            start += re._states.Count;

        }

        int end = _states.Count;
        _states.Add(end);

        foreach (var final in finals)
            AddOrGetResultTransition(final).Add((CharClass.Epsilon, end));

        _finalStates.Add(end);

        _body = distinct.ToArray();

    }

    private protected override int CompareToCore(ThompsonRegex other) {

        Debug.Assert(other is UnionRegex);

        var unionRegex = (UnionRegex)other;

        if (_body.Length > unionRegex._body.Length) return -1;

        if (_body.Length < unionRegex._body.Length) return 1;

        int length = _body.Length;

        for (int i = 0; i < length; ++i) {

            var compare = _body[i].CompareTo(unionRegex._body[i]);

            if (compare is not 0) return compare;

        }

        return 0;
    }

    internal override void Show(StringBuilder builder, int precedence = 0) {

        ReadOnlySpan<ThompsonRegex> bodySpan = _body;

        if (bodySpan.IsEmpty) return;

        builder.AppendIf(precedence > 0, '(');

        bodySpan[0].Show(builder, 0);

        for (int i = 1; i < bodySpan.Length; ++i) {
            builder.Append('|');
            bodySpan[i].Show(builder, 0);
        }

        builder.AppendIf(precedence > 0, ')');

    }

    public override CharClass[] Classy() {

        Debug.Assert(_body.Length > 1);

        ReadOnlySpan<CharClass> result = _body[0].Classy();

        for (int i = 1; i < _body.Length; ++i) {
            result = result.AllPairs(_body[i].Classy());
        }

        return result.ToArray();

    }
}
