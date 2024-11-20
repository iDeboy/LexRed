using System.Collections.Frozen;

namespace LexRed.Brzozowski;

using State = int;
using TransitionFrom = (int State, CharClass Symbols);
using TransitionTo = int;

public sealed class DFA {

    private readonly State _initialState;
    private readonly FrozenSet<State> _states;
    private readonly SortedDictionary<TransitionFrom, TransitionTo> _transitions;
    private readonly FrozenSet<State> _finalStates;

    public DFA(State initialState,
        FrozenSet<State> states,
        SortedDictionary<TransitionFrom, TransitionTo> transitions,
        FrozenSet<State> finalStates) {
        _initialState = initialState;
        _states = states;
        _transitions = transitions;
        _finalStates = finalStates;
    }

    private CharClass FindCharClass(State state, char ch) {

        foreach (var (x, y) in _transitions.Keys) {

            if (x == state && y.Contains(ch)) return y;

        }

        return CharClass.Any;
    }

    public bool IsMatch(ReadOnlySpan<char> input) {

        State currentState = _initialState;

        foreach (ref readonly var ch in input) {

            var cc = FindCharClass(currentState, ch);

            currentState = _transitions[(currentState, cc)];

        }

        return _finalStates.Contains(currentState);

    }
}
