using RdClient.Shared.Models;
namespace RdClient.Shared.ViewModels
{
    /// <summary>
    /// Interface of a task that the EditCredentialsView must perform.
    /// An object implementing the interface is passed to EditCredentialsViewModel by the navigation service
    /// as the presentation parameter.
    /// </summary>
    public interface IEditCredentialsTask
    {
        void PopulateViewModel(IEditCredentialsViewModel viewModel);
        void ValidateViewModel(IEditCredentialsViewModel viewModel);
    }
}
