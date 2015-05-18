using System.Windows.Input;
using RdClient.Shared.CxWrappers;

namespace RdClient.Shared.ViewModels
{
    public interface ICertificateValidationViewModel
    {
        ICommand AcceptCertificate { get; }
        ICommand Cancel { get; }        
        ICommand ShowDetails { get; }
        ICommand HideDetails { get; }            
        IRdpCertificate Certificate { get; }
        string Host { get; }        
        bool RememberChoice { get; set; }
        bool IsExpandedView { get; set; }
    }
}
