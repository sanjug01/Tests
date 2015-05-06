// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModalStackContainer : UserControl
    {
        private IPendingAnimation _pendingAnimation;

        public event EventHandler PushingFirstView;
        public event EventHandler DismissedLastView;

        public ModalStackContainer()
        {
            this.InitializeComponent();
            this.SharedVisualStates.CurrentStateChanging += this.OnVisualStateChanging;
        }

        /// <summary>
        /// Push a UI element to the top of the stack
        /// </summary>
        /// <param name="view">UI element (view) to be shown at the top of the stack</param>
        public void Push(UIElement view, bool animated)
        {
            Contract.Requires(null != view);
            Contract.Ensures(null != _pendingAnimation);

            if (null != _pendingAnimation)
                _pendingAnimation.Commit();

            ContentControl cc = new ContentControl()
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch,
                Content = view
            };

            if (0 == this.RootGrid.Children.Count && null != this.PushingFirstView)
                this.PushingFirstView(this, EventArgs.Empty);
            //
            // Apply the current visual state to the new view on the stack
            //
            if (null != this.SharedVisualStates.CurrentState)
            {
                cc.Content.CastAndCall<Control>(
                    c => VisualStateManager.GoToState(c, this.SharedVisualStates.CurrentState.Name, true));
            }

            if (animated)
            {
                _pendingAnimation = FadeIn.Start(this.RootGrid, cc, this.OnPendingAnimationCompleted);
            }
            else
            {
                this.RootGrid.Children.Add(cc);
            }
        }

        /// <summary>
        /// Remove a UI element from the top of the stack (throw an exception if the specified element
        /// is not at the top).
        /// </summary>
        /// <param name="view">UI element (view) to be removed from the stack</param>
        public void Pop(UIElement view, bool animated)
        {
            Contract.EnsuresOnThrow<Exception>(null == _pendingAnimation);
            Contract.EnsuresOnThrow<ArgumentException>(null == _pendingAnimation);

            if (null != _pendingAnimation)
            {
                _pendingAnimation.Commit();
                _pendingAnimation = null;
            }

            int topChildIndex = this.RootGrid.Children.Count - 1;

            if (topChildIndex >= 0)
            {
                ContentControl cc = this.RootGrid.Children[topChildIndex] as ContentControl;

                if (null == cc)
                    throw new Exception("Control at the top of the stack is not a ContentControl");
                Contract.Assert(object.ReferenceEquals(cc.Content, view));

                if (animated)
                {
                    _pendingAnimation = FadeOut.Start(this.RootGrid, cc, this.OnPendingAnimationCompleted);
                }
                else
                {
                    this.RootGrid.Children.Remove(cc);
                    cc.Content = null;

                    if (0 == this.RootGrid.Children.Count && null != this.DismissedLastView)
                        this.DismissedLastView(this, EventArgs.Empty);
                }
            }
        }

        private void OnPendingAnimationCompleted(IPendingAnimation animation)
        {
            if (object.ReferenceEquals(animation, _pendingAnimation))
            {
                _pendingAnimation = null;
            }

            if (0 == this.RootGrid.Children.Count && null != this.DismissedLastView)
                this.DismissedLastView(this, EventArgs.Empty);
        }

        private void OnVisualStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            //
            // Apply the new visual style to all controls on the stack
            //
            foreach(UIElement ue in this.RootGrid.Children)
            {
                ue.CastAndCall<ContentControl>(
                    cc => cc.Content.CastAndCall<Control>(
                        c => VisualStateManager.GoToState(c, e.NewState.Name, true)));
            }
        }
    }
}
