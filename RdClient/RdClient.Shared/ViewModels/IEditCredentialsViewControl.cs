using System.ComponentModel;
namespace RdClient.Shared.ViewModels
{
    [DefaultValue(EditCredentialsConfirmation.NoMessage)]
    public enum EditCredentialsConfirmation
    {
        NoMessage,
        OverridePassword
    }

    public interface IEditCredentialsViewControl
    {
        /// <summary>
        /// Show the confirmation UI in the credentials editor.
        /// The UI asks user a question, and if user confirms, the credentials editor gets dismissed;
        /// otherwise, nothing happens, the editor remains active and user may make corrections.
        /// </summary>
        /// <param name="message">Identifier of a localized message shown in the confirmation UI.</param>
        /// <remarks>An edit credentials task may call AskConfirmation instead of Submit if data
        /// in the credentials editor is valid but some existing saved values may be overridden.
        /// The result is similar to that of Submit because IEditCredentialsTask.Dismissing
        /// won't be called if user will have confirmed.</remarks>
        void AskConfirmation(EditCredentialsConfirmation message);

        /// <summary>
        /// Dismiss the credentials editor and call IEditCredentialsTask.Dismissed.
        /// </summary>
        void Submit();
    }
}
