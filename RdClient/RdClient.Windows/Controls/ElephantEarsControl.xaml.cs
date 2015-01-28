using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;


namespace RdClient.Controls
{
    public sealed partial class ElephantEarsControl : UserControl
    {
        public static readonly DependencyProperty ElephantEarsVisibleProperty = DependencyProperty.Register(
            "ElephantEarsVisible", typeof(Visibility),
            typeof(ElephantEarsControl), new PropertyMetadata(false, ElephantEarsVisiblePropertyChanged));
        public Visibility ElephantEarsVisible
        {
            private get { return (Visibility)GetValue(ElephantEarsVisibleProperty); }
            set { SetValue(ElephantEarsVisibleProperty, value); }
        }
        private static void ElephantEarsVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ElephantEarsControl control = d as ElephantEarsControl;
            Visibility shownFlag = (Visibility)e.NewValue;
            if (shownFlag == Visibility.Visible)
            {
                control.ShowElephantEarAnimation.Begin();
            }
            else
            {
                control.HideElephantEarAnimation.Begin();
            }
        }

        public ElephantEarsControl()
        {
            this.InitializeComponent();
        }
    }
}
