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
                // Do something
            });
        }
    }
}
