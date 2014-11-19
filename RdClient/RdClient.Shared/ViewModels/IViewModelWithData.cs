using RdClient.Shared.Models;

namespace RdClient.Shared.ViewModels
{
    interface IViewModelWithData
    {
        IDataModel DataModel { get; set; }
    }
}
