using System;
namespace RdClient.Shared.Helpers
{
    public interface IStateMachine<TState, TEvent>
    {
        void AddTransition(TState from, TState to, Predicate<TEvent> predicate, Action<TEvent> action);
        void Consume(TEvent parameter);
        void SetStart(TState start);
    }
}
