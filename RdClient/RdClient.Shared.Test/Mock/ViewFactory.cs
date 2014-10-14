using FadeTest.Navigation;
using System.Collections.Generic;

namespace Test.FadeTest.Shared.Mock
{
    class ViewFactory : IPresentableViewFactory
    {
        private readonly IDictionary<string, PresentableView> _createdViews;

        public ViewFactory()
        {
            _createdViews = new Dictionary<string, PresentableView>();
        }

        public IPresentableView CreateView(string name)
        {
            PresentableView view;
            
            if( !_createdViews.TryGetValue(name, out view) )
            {
                view = new PresentableView(name);
                _createdViews.Add(name, view);
            }
            else
            {
                view.IncrementCreationCount();
            }

            return view;
        }

        public int Count { get { return _createdViews.Count; } }

        public PresentableView GetView(string name)
        {
            return _createdViews[name];
        }
    }
}
