using System.Collections.Frozen;

namespace LexRed.Common;

using State = int;
using TransitionFrom = int;
using TransitionTo = List<(CharClass Symbols, int StateTo)>;

public sealed class NFA : IAutomaton<State, TransitionFrom, TransitionTo> {

    private readonly State _initialState;
    private readonly FrozenSet<State> _states;
    private readonly FrozenDictionary<TransitionFrom, TransitionTo> _transitions;
    private readonly FrozenSet<State> _finalStates;
    private readonly CharClass[] _charClasses;

    public int InitialState => _initialState;

    public FrozenSet<int> States => _states;

    public FrozenDictionary<int, TransitionTo> Transitions => _transitions;

    public FrozenSet<int> FinalStates => _finalStates;

    public NFA(State initialState,
        FrozenSet<State> states,
        FrozenDictionary<TransitionFrom, TransitionTo> transitions,
        FrozenSet<State> finalStates,
        CharClass[] charClasses) {
        _initialState = initialState;
        _states = states;
        _transitions = transitions;
        _finalStates = finalStates;
        _charClasses = charClasses;
    }

    public bool IsMatch(ReadOnlySpan<char> input) {

        HashSet<State> currentStates = [_initialState];

        EpsilonClosure(currentStates);

        foreach (ref readonly var ch in input) {

            var nextStates = new HashSet<State>();

            foreach (var src in currentStates) {

                if (!_transitions.ContainsKey(src)) continue;

                var result = _transitions[src];

                foreach (var (symbols, dst) in result) {

                    if (symbols.Contains(ch)) nextStates.Add(dst);

                }

            }

            currentStates = nextStates;
            EpsilonClosure(currentStates);
        }

        return _finalStates.Overlaps(currentStates);
    }

    private void EpsilonClosure(HashSet<State> states) {

        Stack<State> stack = new(states);

        while (stack.TryPop(out var src)) {

            if (!_transitions.TryGetValue(src, out var to)) continue;

            foreach (var (symbols, dst) in to) {

                if (symbols.IsEpsilon && !states.Contains(dst)) {
                    states.Add(dst);
                    stack.Push(dst);
                }
            }
        }
    }

    private HashSet<State> Move(HashSet<State> states, CharClass symbols) {

        HashSet<State> next = [];

        foreach (var state in states) {

            if (!_transitions.TryGetValue(state, out var to)) continue;

            foreach (var (symbolsTo, dst) in to) {

                if (symbolsTo.Contains(symbols))
                    next.Add(dst);

            }

        }

        return next;
    }

    public DFA ToDFA() {

        HashSet<State> states = [0, 1];
        Dictionary<(State StateFrom, CharClass Symbols), State> transitions = [];
        HashSet<State> finalStates = [];

        var statesMap = new Dictionary<HashSet<int>, int>(HashSet<int>.CreateSetComparer());
        var queue = new Queue<HashSet<int>>();

        HashSet<State> initial = [_initialState];
        EpsilonClosure(initial);

        statesMap[initial] = _initialState;
        queue.Enqueue(initial);

        State nextState = 1;

        while (queue.TryDequeue(out var current)) {

            foreach (var charClass in _charClasses) {

                if (charClass.IsNone) continue;

                var next = Move(current, charClass);
                EpsilonClosure(next);

                // if (next.Count is 0) continue;

                if (statesMap.TryAdd(next, nextState)) {
                    states.Add(nextState++);
                    queue.Enqueue(next);
                }

                transitions.Add((statesMap[current], charClass), statesMap[next]);
            }

            if (_finalStates.Overlaps(current)) {
                finalStates.Add(statesMap[current]);
            }

        }

        return new DFA(_initialState, states.ToFrozenSet(), transitions.ToFrozenDictionary(), finalStates.ToFrozenSet());
    }

}
