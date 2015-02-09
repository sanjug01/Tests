namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Models;
    using RdClient.Shared.ViewModels;
    using System.Diagnostics.Contracts;

    public sealed class DataModelExtension : INavigationExtension
    {
        public ApplicationDataModel AppDataModel { private get; set; }

        public DataModelExtension()
        {
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            Contract.Assert(null != this.AppDataModel);

            viewModel.CastAndCall<IDataModelSite>(vmd =>
            {
                vmd.SetDataModel(this.AppDataModel);
            });
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<IDataModelSite>(vmd =>
            {
                // vmd.DataModel = null;
                vmd.SetDataModel(null);
            });
        }
    }
}
