namespace RdClient.Shared.Navigation
{
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Interface of a site of the application bar.
    /// The bar site is responsible for showing and hiding the bar and for tracking the bar's state.
    /// </summary>
    public interface IApplicationBarSite : INotifyPropertyChanged
    {
        /// <summary>
        /// If true, the application bar does not get dismissed when user interacts with the UI outside of the bar.
        /// </summary>
        bool IsBarSticky { get; set; }
        /// <summary>
        /// Visibility of the application bar. To show or hide the bar consumer of the interface must
        /// execute ShowBar and HodeBar commands.
        /// </summary>
        bool IsBarVisible { get; }
        /// <summary>
        /// Command that shows the application bar.
        /// </summary>
        ICommand ShowBar { get; }
        /// <summary>
        /// Commands that hides the application bar.
        /// </summary>
        ICommand HideBar { get; }
    }
}
