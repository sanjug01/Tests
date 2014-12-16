namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Models;
    using RdClient.Shared.ViewModels;

    public sealed class DataModelExtension : INavigationExtension
    {

        public RdDataModel DataModel { private get; set; }

        public DataModelExtension()
        {
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<IViewModelWithData>(vmd =>
            {
                vmd.DataModel = this.DataModel;
            });
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<IViewModelWithData>(vmd =>
            {
                // vmd.DataModel = null;
            });
        }
    }
}
