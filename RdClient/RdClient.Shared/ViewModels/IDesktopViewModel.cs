using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RdClient.Shared.Models;
using System.Windows.Input;
using System.ComponentModel;

namespace RdClient.Shared.ViewModels
{
    public interface IDesktopViewModel : INotifyPropertyChanged
    {
        DesktopModel Desktop { get; }

        CredentialsModel Credentials { get; }

        ThumbnailModel Thumbnail { get; }

        bool IsSelected { get; set; }

        bool SelectionEnabled { get; set; }

        ICommand EditCommand { get; }

        ICommand ConnectCommand { get; }

        ICommand DeleteCommand { get; }
    }
}
