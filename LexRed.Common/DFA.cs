using System.Collections.Frozen;

namespace LexRed.Common;

using State = int;
using TransitionFrom = (int State, CharClass Symbols);
using TransitionTo = int;

public sealed class DFA : IAutomaton<State, TransitionFrom, TransitionTo> {

    private readonly State _initialState;
    private readonly FrozenSet<State> _states;
    private readonly FrozenDictionary<TransitionFrom, TransitionTo> _transitions;
    private readonly FrozenSet<State> _finalStates;

    public int InitialState => _initialState;

    public FrozenSet<int> States => _states;

    public FrozenDictionary<(int State, CharClass Symbols), int> Transitions => _transitions;

    public FrozenSet<int> FinalStates => _finalStates;

    public DFA(State initialState,
        FrozenSet<State> states,
        FrozenDictionary<TransitionFrom, TransitionTo> transitions,
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
