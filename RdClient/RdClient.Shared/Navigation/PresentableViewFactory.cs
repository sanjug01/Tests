namespace RdClient.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    public sealed class PresentableViewFactory : IPresentableViewFactory
    {
        private readonly IDictionary<string, PresentableViewConstructor> _viewConstructors;

        public static PresentableViewFactory Create()
        {
            return new PresentableViewFactory();
        }

        private PresentableViewFactory()
        {
            _viewConstructors = new Dictionary<string, PresentableViewConstructor>();
        }

        public IPresentableView CreateView(string name, object activationParameter)
        {
            return _viewConstructors[name].CreateView(activationParameter);
        }

        public void AddViewClass( string name, Type viewClass, bool isSingleton = false )
        {
            Contract.Requires(name != null);
            Contract.Requires(viewClass != null);
            _viewConstructors.Add(name, new PresentableViewConstructor(viewClass, isSingleton));
        }

        /// <summary>
        /// Private helper class that creates instances of the specified type and if needed
        /// retains a singleton instance of the type.
        /// </summary>
        class PresentableViewConstructor
        {
            private readonly Type _viewClass;
            private readonly bool _isSingleton;
            private IPresentableView _singletonView;

            public PresentableViewConstructor( Type viewClass, bool isSingleton )
            {
                _viewClass = viewClass;
                _isSingleton = isSingleton;
            }

            public IPresentableView CreateView(object activationParameter)
            {
                IPresentableView newView = _singletonView;

                if( null == newView )
                {
                    newView = Activator.CreateInstance(_viewClass) as IPresentableView;

                    if (null != newView)
                    {
                        newView.Activating(activationParameter);

                        if(_isSingleton)
                         _singletonView = newView;
                    }
                }

                return newView;
            }
        }
    }
}
