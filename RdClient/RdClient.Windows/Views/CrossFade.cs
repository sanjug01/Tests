using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace RdClient.Views
{
    //
    // Interface of a pending animation that may be committed immediately.
    //
    public interface IPendingAnimation
    {
        void Commit();
    }


    public abstract class FadeBase : IPendingAnimation
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

    public sealed class FadeOut : FadeBase
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

            view.Visibility = Visibility.Visible;
            view.Opacity = 1.0;
            view.IsEnabled = false;

            Duration duration = new Duration(TimeSpan.FromSeconds(0.2));
            Timeline tl = new DoubleAnimation() { Duration = duration, From = 1.0, To = 0.0, BeginTime = TimeSpan.FromSeconds(0.0) };
            Storyboard.SetTarget(tl, view);
            Storyboard.SetTargetProperty(tl, "Opacity");
            storyboard.Children.Add(tl);
        }

        protected override void CleanUpAnimation(Grid container, ContentControl view)
        {
            container.Children.Remove(view);
            view.Content = null;
            //
            // If the stack is not empty, enable the top content control on the stack which had been disabled
            // when the removed control was pushed onto the stack.
            //
            int topChildIndex = container.Children.Count - 1;

            if (topChildIndex >= 0)
            {
                ContentControl cc = (ContentControl)container.Children[topChildIndex];
                cc.IsEnabled = true;
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

            view.Visibility = Visibility.Visible;
            view.Opacity = 0.0;
            view.IsEnabled = false;

            container.Children.Add(view);

            Duration duration = new Duration(TimeSpan.FromSeconds(0.2));
            Timeline tl = new DoubleAnimation() { Duration = duration, From = 0.0, To = 1.0, BeginTime = TimeSpan.FromSeconds(0.0) };
            Storyboard.SetTarget(tl, view);
            Storyboard.SetTargetProperty(tl, "Opacity");
            storyboard.Children.Add(tl);
        }

        protected override void CleanUpAnimation(Grid container, ContentControl view)
        {
            view.Opacity = 1.0;
            view.IsEnabled = true;
        }

    }
}
