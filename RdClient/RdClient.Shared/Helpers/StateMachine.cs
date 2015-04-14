using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RdClient.Shared.Helpers
{
    public sealed class StateMachine<TState, TEvent> : IStateMachine<TState,TEvent>
    {
        public sealed class Transition
        {
            public Predicate<TEvent> Predicate { get; set; }
            public TState Destination { get; private set; }
            public Action<TEvent> Action { get; private set; }

            public Transition(Predicate<TEvent> predicate, TState destination, Action<TEvent> action)
            {
                Predicate = predicate;
                Destination = destination;
                Action = action;
            }
        }

        private readonly Dictionary<TState, ICollection<Transition>> _transitions = new Dictionary<TState, ICollection<Transition>>();

        private TState _state;

        public void SetStart(TState start)
        {
            _state = start;
        }

        public void AddTransition(TState from, TState to, Predicate<TEvent> predicate, Action<TEvent> action)
        {
            //Debug.WriteLine("Adding: " + from + " to " + to);

            if(_transitions.ContainsKey(from) == false)
            {
                _transitions[from] = new List<Transition>();
            }

            _transitions[from].Add(new Transition(predicate, to, action));
        }

        public void Consume(TEvent parameter)
        {
            ICollection<Transition> transitions = _transitions[_state];

            foreach(Transition transition in transitions)
            {
                //Debug.WriteLine("Trying: " + _state + " to " + transition.Destination);
                if (transition.Predicate(parameter))
                {
                    transition.Action(parameter);
                    _state = transition.Destination;
                    //Debug.WriteLine(_state);
                    return;
                }
            }
         }
    }
}
