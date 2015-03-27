using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RdClient.DesignTime
{
    class FakeRemoteResourceViewModel : IRemoteResourceViewModel
    {
        public string Name
        {
            get { return "Fake Remote Resource"; }
        }

        public System.Windows.Input.ICommand ConnectCommand
        {
            get { return null; }
        }

        public Windows.UI.Xaml.Media.Imaging.BitmapImage Icon
        {
            get { return null; }
        }
    }
}
