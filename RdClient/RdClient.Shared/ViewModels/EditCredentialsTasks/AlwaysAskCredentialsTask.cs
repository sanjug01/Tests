namespace RdClient.Shared.ViewModels.EditCredentialsTasks
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Edit credentials task that populates the form with values from the input credntials object,
    /// saves any changes made by user in the same object and returns an ID of the user name
    /// if it was found in the data model.
    /// </summary>
    public sealed class AlwaysAskCredentialsTask : IEditCredentialsTask
    {
        private readonly IValidationRule _userNameRule;
        private readonly string _connectionName;
        private readonly CredentialsModel _credentials;
        private readonly ApplicationDataModel _dataModel;

        private bool _passwordChanged;
        private Guid _credentialsId;
        private string _lastKnownUserName;
        private string _lastKnownPassword;

        public event EventHandler<SessionCredentialsEventArgs> Submitted;
        public event EventHandler Cancelled;

        public AlwaysAskCredentialsTask(string connectionName, CredentialsModel credentials, ApplicationDataModel dataModel)
        {
            Contract.Assert(null != connectionName);
            Contract.Assert(null != credentials);
            Contract.Assert(null != dataModel);
            Contract.Ensures(null != _userNameRule);

            _userNameRule = new UsernameValidationRule();
            _connectionName = connectionName;
            _credentials = credentials;
            _dataModel = dataModel;
        }

        void IEditCredentialsTask.Populate(IEditCredentialsViewModel viewModel)
        {
            viewModel.UserName = _credentials.Username;
            viewModel.Password = _credentials.Password;
            viewModel.CanSaveCredentials = true;
            viewModel.DismissLabel = "d:Connect";
            viewModel.ResourceName = _connectionName;
            viewModel.Prompt = "d:Remote connection is set to always ask for credentials";

            _lastKnownUserName = _credentials.Username;
            _lastKnownPassword = _credentials.Password;
        }

        bool IEditCredentialsTask.Validate(IEditCredentialsViewModel viewModel)
        {
            bool valid = IsUserNameValid(viewModel.UserName);

            if (!string.Equals(viewModel.UserName, _lastKnownUserName, StringComparison.OrdinalIgnoreCase))
            {
                //
                // User name changed
                //
                _lastKnownUserName = viewModel.UserName;
            }
            else if( !string.Equals(viewModel.Password, _lastKnownPassword) )
            {
                //
                // Password changed
                //
                _lastKnownPassword = viewModel.Password;
                _passwordChanged = true;
            }

            return valid;
        }

        bool IEditCredentialsTask.Dismissing(IEditCredentialsViewModel viewModel, Action dismiss)
        {
            if(LookUpCredentialsId(viewModel.UserName, out _credentialsId)
            && viewModel.SaveCredentials
            && (AreTrimmedStringsDifferent(viewModel.UserName, _credentials.Username) || _passwordChanged))
            {
                //
                // TODO: user wants to save the password, credentials object is in the data model, and
                //       either the password or user name was changed, ask user if she wants to override
                //       the password.
                //
            }

            return true;
        }

        void IEditCredentialsTask.Dismissed(IEditCredentialsViewModel viewModel)
        {
            _credentials.Username = viewModel.UserName.EmptyIfNull().Trim();
            _credentials.Password = viewModel.Password;

            if (null != this.Submitted)
            {
                this.Submitted(this, new SessionCredentialsEventArgs(_credentials,
                    _credentialsId,
                    viewModel.SaveCredentials));
            }
        }

        void IEditCredentialsTask.Cancelled(IEditCredentialsViewModel viewModel)
        {
            if(null != this.Cancelled)
                this.Cancelled(this, EventArgs.Empty);
        }

        private bool LookUpCredentialsId(string userName, out Guid credentialsId)
        {
            bool found = false;

            userName = userName.EmptyIfNull().Trim();
            credentialsId = Guid.Empty;

            foreach(IModelContainer<CredentialsModel> container in _dataModel.LocalWorkspace.Credentials.Models)
            {
                if(userName.Equals(container.Model.Username.EmptyIfNull().Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    credentialsId = container.Id;
                    found = true;
                }
            }

            return found;
        }

        private static bool AreTrimmedStringsDifferent(string x, string y)
        {
            return !string.Equals(x.EmptyIfNull().Trim(), y.EmptyIfNull().Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private bool IsUserNameValid(string userName)
        {
            userName = userName.EmptyIfNull().Trim();

            bool valid = !string.IsNullOrEmpty(userName);

            return valid;
        }
    }
}
