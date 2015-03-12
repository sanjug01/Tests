namespace RdClient.Shared.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Optional interface that may be implemented by presentable views to track progress of presentation
    /// and dismissing animations. The view presenter (IViewPresenter) will call methods of this interface
    /// to let the views know that they are being animated.
    /// </summary>
    public interface IPresentationAnimation
    {
        void AnimatingIn();
        void AnimatingOut();
        void AnimatedIn();
        void AnimatedOut();
    }
}
