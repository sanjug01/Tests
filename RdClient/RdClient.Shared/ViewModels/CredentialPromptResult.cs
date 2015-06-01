namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;

    public sealed class CredentialPromptResult
    {
        private CredentialPromptResult()
        {
            this.UserCancelled = true;
            this.Deleted = false;
        }

        private CredentialPromptResult(IModelContainer<CredentialsModel> credentials, bool saved)
        {
            this.Credentials = credentials;
            this.Saved = saved;
            this.UserCancelled = false;
            this.Deleted = false;
        }

        public static CredentialPromptResult CreateWithCredentials(IModelContainer<CredentialsModel> credentials, bool saved)
        {
            return new CredentialPromptResult(credentials, saved);
        }

        public static CredentialPromptResult CreateCancelled()
        {
            return new CredentialPromptResult();
        }

        public static CredentialPromptResult CreateDeleted()
        {
            return new CredentialPromptResult() { Deleted = true };
        }

        public IModelContainer<CredentialsModel> Credentials { get; private set; }

        public bool Saved { get; private set; }

        public bool UserCancelled { get; private set; }

        public bool Deleted { get; private set; }
    }
}