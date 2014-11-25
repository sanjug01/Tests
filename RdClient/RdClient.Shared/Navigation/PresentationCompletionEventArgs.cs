namespace RdClient.Shared.Navigation
{
    /// <summary>
    /// Argument for events emitted by the IPresentationCompletion interface.
    /// </summary>
    public sealed class PresentationCompletionEventArgs
    {
        private readonly IPresentableView _view;
        private readonly object _result;

        public IPresentableView View { get { return _view; } }
        public object Result { get { return _result; } }

        public PresentationCompletionEventArgs(IPresentableView view, object result)
        {
            _view = view;
            _result = result;
        }
    }
}
