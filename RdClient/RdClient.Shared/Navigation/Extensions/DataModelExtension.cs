namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Models;
    using RdClient.Shared.ViewModels;
    using System.Diagnostics.Contracts;

    public sealed class DataModelExtension : INavigationExtension
    {
        public ApplicationDataModel AppDataModel { private get; set; }

        public RdDataModel DataModel { private get; set; }

        public DataModelExtension()
        {
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            Contract.Assert(null != this.DataModel);

            viewModel.CastAndCall<IViewModelWithData>(vmd =>
            {
                vmd.DataModel = this.DataModel;
                vmd.SetDataModel(this.AppDataModel);
            });
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<IViewModelWithData>(vmd =>
            {
                // vmd.DataModel = null;
                vmd.SetDataModel(null);
            });
        }
    }
}
