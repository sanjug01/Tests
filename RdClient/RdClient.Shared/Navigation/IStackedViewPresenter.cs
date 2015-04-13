namespace RdClient.Shared.Navigation
{
    using System;

    /// <summary>
    /// Interface of a stacked view presenter - a container that shows a stack of overlapping views.
    /// Only the view at the top of the stack receives user input.
    /// </summary>
    public interface IStackedViewPresenter
    {
        /// <summary>
        /// Pushes a view onto the stack of views.
        /// </summary>
        /// <param name="view">Presentable view to push noto the stack.</param>
        void PushView(IPresentableView view, bool animated);
        /// <summary>
        /// Dismisses the presented view and all views above it on the stack,
        /// </summary>
        /// <param name="view">Presentable view to be dismissed and remove from the stack.</param>
        void DismissView(IPresentableView view, bool animated);
    }
}
