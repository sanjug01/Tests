namespace RdClient.Shared.Navigation
{
    using RdClient.Shared.ViewModels;

    public sealed class DataModelExtension : INavigationExtension
    {
        public DataModelExtension()
        {
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<IViewModelWithData>(vmd =>
            {
                // Deliver the data model to the view model
            });
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<IViewModelWithData>(vmd =>
            {
                // Remove the data model from the view model
            });
        }
    }
}
