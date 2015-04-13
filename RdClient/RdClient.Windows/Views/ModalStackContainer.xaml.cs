// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RdClient.Views
{
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

        public ModalStackContainer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Push a UI element to the top of the stack
        /// </summary>
        /// <param name="view">UI element (view) to be shown at the top of the stack</param>
        public void Push(UIElement view)
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

            _pendingAnimation = FadeIn.Start(this.RootGrid, cc, this.OnPendingAnimationCompleted);
        }

        /// <summary>
        /// Remove a UI element from the top of the stack (throw an exception if the specified element
        /// is not at the top).
        /// </summary>
        /// <param name="view">UI element (view) to be removed from the stack</param>
        public void Pop(UIElement view)
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

                _pendingAnimation = FadeOut.Start(this.RootGrid, cc, this.OnPendingAnimationCompleted);
            }
        }

        private void OnPendingAnimationCompleted(IPendingAnimation animation)
        {
            if (object.ReferenceEquals(animation, _pendingAnimation))
            {
                _pendingAnimation = null;
            }
        }

    }
}
