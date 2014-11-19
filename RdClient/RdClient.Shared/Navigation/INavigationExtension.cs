using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Navigation
{
    public class NavigationExtensionList : List<INavigationExtension>
    {
    }

    public interface INavigationExtension
    {
        void Presenting(IViewModel viewModel);
    }
}
