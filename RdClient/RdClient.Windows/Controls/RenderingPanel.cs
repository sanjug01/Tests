namespace RdClient.Controls
{
    using RdClient.Shared.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Wrapper of SwapChainPanel that adds the IRenderingPanel interface.
    /// </summary>
    public sealed class RenderingPanel : SwapChainPanel, IRenderingPanel
    {
        private EventHandler _ready;

        public RenderingPanel()
        {
            //
            // Explicitly make the default constructor public.
            //
            this.SizeChanged += this.OnSizeChanged;
        }

        event EventHandler IRenderingPanel.Ready
        {
            add
            {
                _ready += value;

                if (this.ActualHeight > 0 && this.ActualWidth > 0)
                    value(this, EventArgs.Empty);
            }
            remove { _ready -= value; }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((e.PreviousSize.Width == 0 || e.PreviousSize.Height == 0) && e.NewSize.Width > 0 && e.NewSize.Height > 0)
            {
                if (null != _ready)
                    _ready(this, EventArgs.Empty);
            }
        }
    }
}
