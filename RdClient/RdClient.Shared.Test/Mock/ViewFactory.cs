using RdClient.Navigation;
using System.Collections.Generic;

namespace Test.RdClient.Shared.Mock
{
    class ViewFactory : IPresentableViewFactory
    {
        private readonly IDictionary<string, PresentableView> _createdViews;

        public ViewFactory()
        {
            _createdViews = new Dictionary<string, PresentableView>();
        }

        public virtual IPresentableView CreateView(string name, object activationParameter)
        {
            PresentableView view;
            
            if( !_createdViews.TryGetValue(name, out view) )
            {
                view = new PresentableView(name);
                _createdViews.Add(name, view);

                view.Activating(activationParameter);
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


        public void AddViewClass(string name, System.Type viewClass, bool isSingleton = false)
        {
            throw new System.NotImplementedException();
        }
    }
}
