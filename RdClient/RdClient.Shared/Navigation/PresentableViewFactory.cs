namespace RdClient.Navigation
{
    using RdClient.Shared.Navigation;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    public sealed class PresentableViewFactory<TPresentableViewConstructor> : IPresentableViewFactory where TPresentableViewConstructor : IPresentableViewConstructor, new()
    {
        private readonly IDictionary<string, IPresentableViewConstructor> _viewConstructors;


        public PresentableViewFactory()
        {
            _viewConstructors = new Dictionary<string, IPresentableViewConstructor>();
        }

        public IPresentableView CreateView(string name, object activationParameter)
        {
            IPresentableView newView = _viewConstructors[name].CreateView();

            if (newView == null)
            {
                throw new NavigationServiceException("Tried to create unknown view: " + name);
            }

            newView.Activating(activationParameter);
            return newView;
        }

        public void AddViewClass(string name, Type viewClass, IPresentableViewConstructor pvc, bool isSingleton)
        {
            Contract.Requires(name != null && !name.Equals(""));
            Contract.Requires(viewClass != null);
            pvc.Initialize(viewClass, isSingleton);
            _viewConstructors.Add(name, pvc);
        }

        public void AddViewClass(string name, Type viewClass, bool isSingleton = false)
        {
            AddViewClass(name, viewClass, new TPresentableViewConstructor(), isSingleton);
        }
    }
}
