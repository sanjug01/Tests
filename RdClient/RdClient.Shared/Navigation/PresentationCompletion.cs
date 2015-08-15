using System;

namespace RdClient.Shared.Navigation
{
    public class PresentationCompletion : IPresentationCompletion
    {
        public readonly Action<IPresentableView, object> _action;

        public void Completed(IPresentableView view, object result)
        {
            if(_action != null)
            {
                _action(view, result);
            }
        }

        public PresentationCompletion(Action<IPresentableView, object> action)
        {
            _action = action;
        }
    }
}
