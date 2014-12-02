namespace RdClient.Helpers
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;
    using Windows.UI.Core;
    using Windows.UI.Xaml;

    /// <summary>
    /// Implementation of IDeferredExecution that uses the CoreDispatcher class to defer
    /// execution of actionsto the UI thread.
    /// </summary>
    /// <remarks>The object is designed to be created in XAML and passed to navigation
    /// extensions that use IDeferredExecution.</remarks>
    sealed class CoreDispatcherDeferredExecution : DependencyObject, IDeferredExecution
    {
        /// <summary>
        /// Dependency property specifying the priority used to post actions to the dispatcher.
        /// </summary>
        public readonly DependencyProperty PriorityProperty = DependencyProperty.Register("Priority",
            typeof(CoreDispatcherPriority), typeof(CoreDispatcherDeferredExecution),
            new PropertyMetadata(CoreDispatcherPriority.Normal,
                (s, e) => s.CastAndCall<CoreDispatcherDeferredExecution>(disp => disp.CachePriority((CoreDispatcherPriority)e.NewValue))));
        //
        // Cached value of the Priority dependency property used to call the dispatcher
        // on worker threads. The cached value is updated by the property change callback
        // of the dependency property.
        //
        private volatile CoreDispatcherPriority _cachedPriotity;

        /// <summary>
        /// Convenience accessor property for the PriorityProperty dependency property.
        /// </summary>
        public CoreDispatcherPriority Priority
        {
            get { return (CoreDispatcherPriority)GetValue(PriorityProperty); }
            set { SetValue(PriorityProperty, value); }
        }

        void IDeferredExecution.Defer(Action action)
        {
            Contract.Requires(null != action);

            if (this.Dispatcher.HasThreadAccess)
            {
                //
                // If the call is made on the UI thread, execute the action immediately.
                //
                action();
            }
            else
            {
                //
                // Post the action for execution with the configured priority.
                //
                var ignore = this.Dispatcher.RunAsync(_cachedPriotity, () => action());
            }
        }

        private void CachePriority(CoreDispatcherPriority priority)
        {
            _cachedPriotity = priority;
        }
    }
}
