namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Windows.Input;

    public interface IEditCredentialsViewModel
    {
        ICommand Cancel { get; }
        ICommand Dismiss { get; }
        ICommand Confirm { get; }
        ICommand CancelConfirmation { get; }

        bool CanRevealPassword { get; set; }

        bool CanSaveCredentials { get; set; }

        bool ShowPrompt { get; set; }

        CredentialPromptMode PromptMode { get; set; }

        string ResourceName { get; set; }

        bool SaveCredentials { get; set; }

        string UserName { get; set; }

        string Password { get; set; }

        bool IsConfirmationVisible { get; }

        EditCredentialsConfirmation ConfirmationMessage { get; }
    }
}
