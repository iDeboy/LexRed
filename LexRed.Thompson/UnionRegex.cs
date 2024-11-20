using LexRed.Common;

namespace LexRed.Thompson;
public class UnionRegex : ThompsonRegex {

    public override ThompsonRegexKind Kind => ThompsonRegexKind.Union;

    public UnionRegex(params Span<ThompsonRegex> body) {

        const int start = 0;

        _transitions.Add(start, [(CharClass.Epsilon, 1)]);

        foreach (var re in body) {

            var result = re._transitions[start];

            List<(CharClass Symbols, int StateTo)> map = [];

            foreach (var (symbols, stateTo) in result) {

                map.Add((symbols, stateTo + 1));

            }

            _transitions.Add(1, map);

        }

    }
}
