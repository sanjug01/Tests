namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Input.Keyboard;
    using System;
    using System.Diagnostics.Contracts;

    public class InputPanelFactoryExtension : INavigationExtension
    {
        private readonly IInputPanelFactory _inputPanelFactory;

        public InputPanelFactoryExtension(IInputPanelFactory inputPanelFactory)
        {
            Contract.Assert(null != inputPanelFactory);
            _inputPanelFactory = inputPanelFactory;
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<IInputPanelFactorySite>(site => site.SetInputPanelFactory(null));
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<IInputPanelFactorySite>(site => site.SetInputPanelFactory(_inputPanelFactory));
        }
    }
}
