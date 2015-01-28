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
        public static readonly DependencyProperty ElephantEarsShownProperty = DependencyProperty.Register(
            "ElephantEarsShown", typeof(bool),
            typeof(ElephantEarsControl), new PropertyMetadata(false, ElephantEarsShownPropertyChanged));
        public bool ElephantEarsShown
        {
            private get { return (bool)GetValue(ElephantEarsShownProperty); }
            set { SetValue(ElephantEarsShownProperty, value); }
        }
        private static void ElephantEarsShownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ElephantEarsControl control = d as ElephantEarsControl;
            bool shownFlag = (bool)e.NewValue;
            if (shownFlag)
            {
                (control.Resources["ShowElephantEarAnimation"] as Storyboard).Begin();
            }
            else
            {
                (control.Resources["HideElephantEarAnimation"] as Storyboard).Begin();
            }
        }

        public ElephantEarsControl()
        {
            this.InitializeComponent();
        }
    }
}
