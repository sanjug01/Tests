using RdClient.Navigation;

namespace Test.RdClient.Shared.Mock
{
    class ViewPresenter : IViewPresenter
    {
        private int _presentViewCount = 0;

        public int PresentViewCount
        {
            get { return _presentViewCount; }
        }
        private int _pushModalViewCount = 0;

        public int PushModalViewCount
        {
            get { return _pushModalViewCount; }
        }
        private int _dismissModalViewcount = 0;

        public int DismissModalViewcount
        {
            get { return _dismissModalViewcount; }
        }

        public ViewPresenter()
        {
        }

        public void PresentView(IPresentableView view)
        {
            _presentViewCount++;
        }

        public void PushModalView(IPresentableView view)
        {
            _pushModalViewCount++;
        }

        public void DismissModalView(IPresentableView view)
        {
            _dismissModalViewcount++;
        }
    }
}
