using RdClient.Navigation;
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
using Windows.UI.Xaml.Navigation;

namespace RdClient
{
    public sealed partial class MainPage : Page, IViewPresenter
    {
        private UIElement _currentElement;
        private UIElement _newElement;

        public MainPage()
        {
            this.InitializeComponent();
        }

        public void PresentView(IPresentableView view)
        {
            throw new NotImplementedException();
        }

        public void PushModalView(IPresentableView view)
        {
            throw new NotImplementedException();
        }

        public void DismissModalView(IPresentableView view)
        {
            throw new NotImplementedException();
        }
    }
}
