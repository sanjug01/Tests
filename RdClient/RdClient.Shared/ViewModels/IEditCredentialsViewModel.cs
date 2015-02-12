namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Windows.Input;

    public interface IEditCredentialsViewModel
    {
        ICommand Cancel { get; }
        ICommand Dismiss { get; }
        string DismissLabel { get; set; }

        bool CanRevealPassword { get; set; }

        bool CanSaveCredentials { get; set; }

        string Prompt { get; set; }

        string ResourceName { get; set; }

        bool SaveCredentials { get; set; }

        string UserName { get; set; }

        string Password { get; set; }
    }
}
