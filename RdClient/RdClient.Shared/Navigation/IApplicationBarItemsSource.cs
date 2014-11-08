namespace RdClient.Shared.Navigation
{
    using RdClient.Shared.ViewModels;
    using System.Collections.Generic;

    /// <summary>
    /// Interface of an object that provides models for items that may be shown in the application bar.
    /// The interface may be implemented by a navigation subject (view or view model, preferrably the view model).
    /// The navigation service will retrieve a collection of bar item models upon activation of a new navigation subject,
    /// and will populate the application bar with controls based on the models.
    /// </summary>
    public interface IApplicationBarItemsSource
    {
        /// <summary>
        /// Return a collection of bar item models.
        /// </summary>
        /// <param name="applicationBarSite">Object responsible for showing and hiding the application bar. Navigation subject
        /// may cache the object but must release it upon dismissing.</param>
        /// <returns>Enumerable collection of bar item model objects. If the returned collection is empty or the returned
        /// object is null, the navigation service will not enable the application bar and execution of commands
        /// in the application bar site object will do nothing.</returns>
        IEnumerable<BarItemModel> GetItems(IApplicationBarSite applicationBarSite);
    }
}
