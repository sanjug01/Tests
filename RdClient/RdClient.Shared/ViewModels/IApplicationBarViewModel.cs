namespace RdClient.Shared.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;

    public interface IApplicationBarViewModel : INotifyPropertyChanged
    {
        IEnumerable<BarItemModel> BarItems { get; set; }
        /// <summary>
        /// Property tracks visibility of the clickable control that executes the ShowBar command.
        /// </summary>
        bool IsShowBarButtonVisible { get; set; }
        /// <summary>
        /// Property tracks visibility of the application bar.
        /// </summary>
        bool IsBarVisible { get; set; }
        /// <summary>
        /// Property tracks stickiness of the application bar. When the bar is sticky it is not hidden
        /// when user interacts with the UI outside of the bar.
        /// </summary>
        bool IsBarSticky { get; set; }
        /// <summary>
        /// Command that shows the application bar.
        /// </summary>
        ICommand ShowBar { get; }
    }
}
