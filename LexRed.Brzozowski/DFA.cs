namespace LexRed.Brzozowski;

using TransitionFrom = (int State, char Symbol);
using State = int;
using TransitionTo = int;

public sealed class DFA {

    private readonly State _initialState;
    private readonly HashSet<State> _states;
    private readonly Dictionary<TransitionFrom, TransitionTo> _transitions;
    private readonly HashSet<State> _finalStates;

    public DFA(State initialState, 
        HashSet<State> states, 
        Dictionary<TransitionFrom, TransitionTo> transitions,
        HashSet<State> finalStates) {
        _initialState = initialState;
        _states = states;
        _transitions = transitions;
        _finalStates = finalStates;
    }

}
