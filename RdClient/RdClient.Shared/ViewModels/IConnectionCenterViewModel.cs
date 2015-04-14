﻿namespace RdClient.Shared.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public interface IConnectionCenterViewModel
    {
        RelayCommand AddDesktopCommand { get; }
        ReadOnlyObservableCollection<IDesktopViewModel> DesktopViewModels { get; }
        ReadOnlyObservableCollection<IWorkspaceViewModel> WorkspaceViewModels { get; }

        /// <summary>
        /// Collection of items shown in the toolbar.
        /// </summary>
        /// <remarks>Location of the toolbar may be different for different screen sizes and orientations.</remarks>
        ReadOnlyObservableCollection<BarItemModel> ToolbarItems { get; }

        bool HasDesktops { get; }
        bool HasApps { get; }
        bool ShowDesktops { get; set; }
        bool ShowApps { get; set; }

        bool IsAccessoryViewPresented { get; }
        ICommand CancelAccessoryView { get; }
    }
}
