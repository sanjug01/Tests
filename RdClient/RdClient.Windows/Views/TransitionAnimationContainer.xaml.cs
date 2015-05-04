namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;

    public sealed partial class TransitionAnimationContainer : UserControl
    {
        private Storyboard _storyboard;
        private UIElement _currentContent, _newContent;

        public TransitionAnimationContainer()
        {
            this.InitializeComponent();
            this.SharedVisualStates.CurrentStateChanging += this.OnSharedVisualStateChanging;
        }

        public void ShowContent(UIElement content)
        {
            Contract.Requires(null != content);
            //
            // Apply the current shared visual state before presenting the new element.
            //
            if (null != this.SharedVisualStates.CurrentState)
            {
                _currentContent.CastAndCall<Control>(
                    c => VisualStateManager.GoToState(c, this.SharedVisualStates.CurrentState.Name, true));
            }

            if (null != _currentContent)
            {
                //
                // Begin cross-fade animation from the current to the new content.
                //
                CommitPendingAnimation(true);
                _newContent = content;

                _storyboard = new Storyboard();
                _storyboard.Completed += OnCrossFadeCompleted;

                Duration duration = new Duration(TimeSpan.FromSeconds(0.2));
                Timeline tl = new DoubleAnimation() { Duration = duration, From = 0.0, To = 1.0, BeginTime = TimeSpan.FromSeconds(0.0) };
                Storyboard.SetTarget(tl, content);
                Storyboard.SetTargetProperty(tl, "Opacity");
                _storyboard.Children.Add(tl);

                tl = new DoubleAnimation() { Duration = duration, From = 1.0, To = 0.0, BeginTime = TimeSpan.FromSeconds(0.0) };
                Storyboard.SetTarget(tl, _currentContent);
                Storyboard.SetTargetProperty(tl, "Opacity");
                _storyboard.Children.Add(tl);

                _newContent.Opacity = 0.0;
                _newContent.Visibility = Visibility.Visible;
                this.ContentGrid.Children.Add(_newContent);

                _currentContent.CastAndCall<IPresentationAnimation>(a => a.AnimatingOut());
                _newContent.CastAndCall<IPresentationAnimation>(a => a.AnimatingIn());
                this.RootContainer.IsHitTestVisible = false;
                _storyboard.Begin();
            }
            else
            {
                //
                //  There is no current content, simply show the content in the grid.
                //
                content.Visibility = Visibility.Visible;
                content.Opacity = 1.0;
                content.CastAndCall<IPresentationAnimation>(a => a.AnimatingIn());
                this.ContentGrid.Children.Add(content);
                _currentContent = content;
                content.CastAndCall<IPresentationAnimation>(a => a.AnimatedIn());
            }
        }

        private void CommitPendingAnimation(bool stopStoryboard)
        {
            if(null != _storyboard)
            {
                if (stopStoryboard)
                    _storyboard.Stop();
                _storyboard.Completed -= OnCrossFadeCompleted;
                _storyboard = null;
                this.ContentGrid.Children.Remove(_currentContent);
                _currentContent = _newContent;
                _currentContent.Opacity = 1.0;
                this.RootContainer.IsHitTestVisible = true;
                _newContent.CastAndCall<IPresentationAnimation>(a => a.AnimatedOut());
                _currentContent.CastAndCall<IPresentationAnimation>(a => a.AnimatedIn());
                _newContent = null;
            }
        }

        private void OnCrossFadeCompleted(object sender, object e)
        {
            CommitPendingAnimation(false);
        }

        private void OnSharedVisualStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            CommitPendingAnimation(true);
            _currentContent.CastAndCall<Control>(c => VisualStateManager.GoToState(c, e.NewState.Name, true));
        }
    }
}
