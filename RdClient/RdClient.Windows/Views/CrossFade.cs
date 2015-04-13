namespace RdClient.Views
{
    using RdClient.Shared.Navigation;
    using System;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media.Animation;

    //
    // Interface of a pending animation that may be committed immediately.
    //
    interface IPendingAnimation
    {
        void Commit();
    }

    abstract class FadeBase : IPendingAnimation
    {
        private readonly Grid _container;
        private readonly ContentControl _view;
        private readonly Action<IPendingAnimation> _completion;
        private Storyboard _storyboard;

        protected FadeBase(Grid container, ContentControl view, Action<IPendingAnimation> completion)
        {
            Contract.Requires(null != container);
            Contract.Requires(null != view);
            Contract.Requires(null != completion);
            Contract.Ensures(null != _container);
            Contract.Ensures(null != _view);
            Contract.Ensures(null != _completion);
            Contract.Ensures(null == _storyboard);

            _container = container;
            _view = view;
            _completion = completion;
        }

        protected void StartAnimation()
        {
            Contract.Requires(null == _storyboard);

            _storyboard = new Storyboard();
            _storyboard.Completed += this.OnCompleted;
            Contract.Assert(null != _storyboard);
            SetUpAnimation(_storyboard, _container, _view);
            _storyboard.Begin();
        }

        protected abstract void SetUpAnimation(Storyboard storyboard, Grid container, ContentControl view);
        protected abstract void CleanUpAnimation(Grid container, ContentControl view);

        private void OnCompleted(object sender, object e)
        {
            Contract.Ensures(null == _storyboard);

            if (null != _storyboard)
            {
                _storyboard.Completed -= this.OnCompleted;
                _storyboard = null;
                CleanUpAnimation(_container, _view);
                _completion(this);
            }
        }

        public void Commit()
        {
            Contract.Ensures(null == _storyboard);

            if (null != _storyboard)
            {
                _storyboard.Completed -= this.OnCompleted;
                _storyboard.Stop();
                _storyboard = null;
                CleanUpAnimation(_container, _view);
            }
        }
    }

    sealed class FadeOut : FadeBase
    {
        public static IPendingAnimation Start(Grid container, ContentControl view, Action<IPendingAnimation> completion)
        {
            Contract.Requires(null != container);
            Contract.Requires(null != view);
            Contract.Requires(null != completion);
            Contract.Ensures(null != Contract.Result<IPendingAnimation>());

            FadeOut animation = new FadeOut(container, view, completion);
            animation.StartAnimation();
            return animation;
        }

        private FadeOut(Grid container, ContentControl view, Action<IPendingAnimation> completion)
            : base(container, view, completion)
        {
            Contract.Requires(null != container);
            Contract.Requires(null != view);
            Contract.Requires(null != completion);
        }

        protected override void SetUpAnimation(Storyboard storyboard, Grid container, ContentControl view)
        {
            Contract.Requires(null != storyboard);
            Contract.Requires(null != container);
            Contract.Requires(null != view);

            view.Content.CastAndCall<IPresentationAnimation>(a => a.AnimatingOut());
            view.Visibility = Visibility.Visible;
            view.Opacity = 1.0;
            view.IsHitTestVisible = false;

            Duration duration = new Duration(TimeSpan.FromSeconds(0.2));
            Timeline tl = new DoubleAnimation() { Duration = duration, From = 1.0, To = 0.0, BeginTime = TimeSpan.FromSeconds(0.0) };
            Storyboard.SetTarget(tl, view);
            Storyboard.SetTargetProperty(tl, "Opacity");
            storyboard.Children.Add(tl);
        }

        protected override void CleanUpAnimation(Grid container, ContentControl view)
        {
            Contract.Requires(null != container);
            Contract.Requires(null != view);

            container.Children.Remove(view);
            view.Content.CastAndCall<IPresentationAnimation>(a => a.AnimatedOut());
            view.Content = null;
            //
            // If the stack is not empty, enable the top content control on the stack which had been disabled
            // when the removed control was pushed onto the stack.
            //
            int topChildIndex = container.Children.Count - 1;

            if (topChildIndex >= 0)
            {
                ContentControl cc = (ContentControl)container.Children[topChildIndex];
                cc.Content.CastAndCall<IPresentationAnimation>(a => a.AnimatingIn());
                cc.IsHitTestVisible = true;
                cc.Content.CastAndCall<IPresentationAnimation>(a => a.AnimatedIn());
            }
        }

    }

    sealed class FadeIn : FadeBase
    {
        public static IPendingAnimation Start(Grid container, ContentControl view, Action<IPendingAnimation> completion)
        {
            Contract.Requires(null != container);
            Contract.Requires(null != view);
            Contract.Requires(null != completion);
            Contract.Ensures(null != Contract.Result<IPendingAnimation>());

            FadeIn animation = new FadeIn(container, view, completion);
            animation.StartAnimation();
            return animation;
        }

        private FadeIn(Grid container, ContentControl view, Action<IPendingAnimation> completion)
            : base(container, view, completion)
        {
            Contract.Requires(null != container);
            Contract.Requires(null != view);
            Contract.Requires(null != completion);
        }

        protected override void SetUpAnimation(Storyboard storyboard, Grid container, ContentControl view)
        {
            Contract.Requires(null != storyboard);
            Contract.Requires(null != container);
            Contract.Requires(null != view);

            IPresentationAnimation previousTop = null;

            view.Content.CastAndCall<IPresentationAnimation>(a => a.AnimatingIn());
            view.Visibility = Visibility.Visible;
            view.Opacity = 0.0;
            view.IsHitTestVisible = false;

            if(container.Children.Count > 0)
            {
                ContentControl cc = container.Children[container.Children.Count - 1] as ContentControl;

                if (null != cc)
                {
                    previousTop = cc.Content as IPresentationAnimation;

                    if(null != previousTop)
                        previousTop.AnimatingOut();
                }
            }

            container.Children.Add(view);

            if (null != previousTop)
                previousTop.AnimatedOut();

            Duration duration = new Duration(TimeSpan.FromSeconds(0.2));
            Timeline tl = new DoubleAnimation() { Duration = duration, From = 0.0, To = 1.0, BeginTime = TimeSpan.FromSeconds(0.0) };
            Storyboard.SetTarget(tl, view);
            Storyboard.SetTargetProperty(tl, "Opacity");
            storyboard.Children.Add(tl);
        }

        protected override void CleanUpAnimation(Grid container, ContentControl view)
        {
            Contract.Requires(null != container);
            Contract.Requires(null != view);

            view.Opacity = 1.0;
            view.IsHitTestVisible = true;
            view.Content.CastAndCall<IPresentationAnimation>(a => a.AnimatedIn());
        }
    }
}
