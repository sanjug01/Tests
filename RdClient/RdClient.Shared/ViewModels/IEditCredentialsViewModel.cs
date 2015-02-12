namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Windows.Input;

    public interface IEditCredentialsViewModel
    {
        ICommand Cancel { get; }
        ICommand Dismiss { get; }
        string DismissLabel { get; }

        bool CanRevealPassword { get; }

        bool CanSaveCredentials { get; }

        string Prompt { get; }

        string ResourceName { get; }

        bool SaveCredentials { get; set; }

        string UserName { get; set; }

        string Password { get; set; }
    }
}
