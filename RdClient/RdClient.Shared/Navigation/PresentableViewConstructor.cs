using System;
using System.Diagnostics.Contracts;

namespace RdClient.Shared.Navigation
{

    /// <summary>
    /// Private helper class that creates instances of the specified type and if needed
    /// retains a singleton instance of the type.
    /// </summary>
    public class PresentableViewConstructor : IPresentableViewConstructor
    {
        private Type _viewClass;
        private bool _isSingleton;
        private IPresentableView _singletonView;

        public void Initialize(Type viewClass, bool isSingleton)
        {
            Contract.Requires(viewClass != null);

            _viewClass = viewClass;
            _isSingleton = isSingleton;
        }

        public IPresentableView CreateView()
        {
            IPresentableView newView = _singletonView;

            if (null == newView)
            {
                newView = Activator.CreateInstance(_viewClass) as IPresentableView;

                if (null != newView)
                {
                    if (_isSingleton)
                        _singletonView = newView;
                }
            }

            return newView;
        }
    }
}
