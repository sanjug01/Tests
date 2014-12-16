using RdClient.Shared.Models;

namespace RdClient.Shared.ViewModels
{
    interface IViewModelWithData
    {
        RdDataModel DataModel { get; set; }
    }
}
