using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Navigation
{
    public class NavigationExtensionList : List<INavigationExtension>
    {
    }

    /// <summary>
    /// Extension object that the navigation service calls when it presents and dismisses view models.
    /// </summary>
    public interface INavigationExtension
    {
        /// <summary>
        /// Called by the navigation service immediately before it will present a view model.
        /// </summary>
        /// <param name="viewModel">View model that the navigation service is about to present.</param>
        void Presenting(IViewModel viewModel);

        /// <summary>
        /// Called by the navigation service immediately after it has dismissed a view model.
        /// </summary>
        /// <param name="viewModel">View model that the navigation service has dismissed.</param>
        void Dismissed(IViewModel viewModel);
    }
}
