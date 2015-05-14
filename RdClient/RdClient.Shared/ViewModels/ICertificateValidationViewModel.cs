using System.Windows.Input;
using RdClient.Shared.CxWrappers;

namespace RdClient.Shared.ViewModels
{
    public interface ICertificateValidationViewModel
    {
        ICommand AcceptCertificateCommand { get; }
        ICommand AcceptOnceCommand { get; }
        ICommand CancelCommand { get; }
        IRdpCertificate Certificate { get; }
        ICommand HideDetailsCommand { get; }
        string Host { get; set; }
        bool IsExpandedView { get; set; }
        ICommand ShowDetailsCommand { get; }
    }
}