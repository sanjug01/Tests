using System;
using RdClient.Navigation;

namespace Test.RdClient.Shared.Mock
{
    class PresentableView : IPresentableView
    {
        private readonly string _name;
        private int _creationCount,
                    _activationCount,
                    _presentationsCount,
                    _dismissalsCount;
        private object _lastActivationParameter;

        public PresentableView(string name)
        {
            _name = name;
            _creationCount = 1;
            _activationCount = 0;
            _presentationsCount = 0;
            _dismissalsCount = 0;
        }

        public PresentableView()
        {
            _name = string.Empty;
            _creationCount = 1;
            _activationCount = 0;
            _presentationsCount = 0;
            _dismissalsCount = 0;
        }

        public string Name { get { return _name; } }
        public int CreationCount { get { return _creationCount; } }
        public int ActivationsCount { get { return _activationCount; } }
        public int PresentationsCount { get { return _presentationsCount; } }
        public int DismissalsCount { get { return _dismissalsCount; } }
        public object LastActivationParameter { get { return _lastActivationParameter; } }

        public void IncrementCreationCount()
        {
            ++_creationCount;
        }

        public void Activating(object activationParameter)
        {
            ++_activationCount;
            _lastActivationParameter = activationParameter;
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            ++_presentationsCount;
            _lastActivationParameter = activationParameter;
        }

        public void Dismissing()
        {
            ++_dismissalsCount;
        }
    }
}
