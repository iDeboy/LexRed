using System.Collections.Frozen;

namespace LexRed.Common;
public interface IAutomaton<TState, TTransitionFrom, TTransitionTo>
    where TTransitionFrom : notnull {

    public TState InitialState { get; }

    public FrozenSet<TState> States { get; }
    public FrozenDictionary<TTransitionFrom, TTransitionTo> Transitions { get; }
    public FrozenSet<TState> FinalStates { get; }

    public bool IsMatch(ReadOnlySpan<char> input);
}
