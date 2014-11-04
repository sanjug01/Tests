using Windows.ApplicationModel;

namespace RdClient.Windows.Test.Mock
{

    class MySuspendingDeferral : ISuspendingDeferral
    {
        private int _completionCount = 0;

        public int CompletionCount { get { return _completionCount; } }

        public void Complete()
        {
            _completionCount += 1;
        }
    }

}
