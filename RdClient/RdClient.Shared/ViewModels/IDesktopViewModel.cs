using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RdClient.Shared.Models;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public interface IDesktopViewModel
    {
        Desktop Desktop { get; }

        Credentials Credential { get; }

        ICommand EditCommand { get; }

        ICommand ConnectCommand { get; }

        ICommand DeleteCommand { get; }
    }
}
