namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Models;

    interface IViewModelWithData
    {
        RdDataModel DataModel { get; set; }
        void SetDataModel(ApplicationDataModel dataModel);
    }
}
