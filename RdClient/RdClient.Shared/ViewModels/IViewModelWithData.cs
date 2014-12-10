using RdClient.Shared.Models;

namespace RdClient.Shared.ViewModels
{
    interface IViewModelWithData
    {
        PersistentData DataModel { get; set; }
    }
}
