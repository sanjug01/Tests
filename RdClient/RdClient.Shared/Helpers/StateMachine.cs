using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RdClient.Shared.Helpers
{
    public class StateMachine
    {
        public delegate bool Predicate(object arg);
        public delegate void Action(object arg);

        public class Transition
        {
            public Predicate Predicate { get; private set; }
            public string Destination { get; private set; }
            public Action Action { get; private set; }

            public Transition(Predicate predicate, string destination, Action action)
            {
                Predicate = predicate;
                Destination = destination;
                Action = action;
            }
        }

        private Dictionary<string, List<Transition>> _transitions =  new Dictionary<string,List<Transition>>();

        private string _state;

        public void SetStart(string start)
        {
            _state = start;
        }

        public void AddTransition(string from, string to, Predicate predicate, Action action)
        {
            if(_transitions.ContainsKey(from) == false)
            {
                _transitions[from] = new List<Transition>();
            }

            _transitions[from].Add(new Transition(predicate, to, action));
        }

        public void DoTransition(object parameter)
        {
            List<Transition> transitions = _transitions[_state];

            foreach(Transition transition in transitions)
            {
                if (transition.Predicate(parameter))
                {
                    transition.Action(parameter);
                    _state = transition.Destination;
                    Debug.WriteLine(_state);
                    return;
                }
            }
        }
    }
}
