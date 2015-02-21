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

        private bool _ignoreModelUpdates;
        private bool _passwordChanged;
        private Guid _credentialsId;

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
            _ignoreModelUpdates = false;
        }

        void IEditCredentialsTask.Populate(IEditCredentialsViewModel viewModel)
        {
            viewModel.UserName = _credentials.Username;
            viewModel.Password = _credentials.Password;
            viewModel.CanSaveCredentials = true;
            viewModel.DismissLabel = "d:Connect";
            viewModel.ResourceName = _connectionName;
            viewModel.Prompt = "d:Remote connection is set to always ask for credentials";
        }

        bool IEditCredentialsTask.Validate(IEditCredentialsViewModel viewModel)
        {
            return IsUserNameValid(viewModel.UserName);
        }

        bool IEditCredentialsTask.ValidateChangedProperty(IEditCredentialsViewModel viewModel, string propertyName)
        {
            bool valid = true;

            if(propertyName.Equals(EditCredentialsViewModel.UserNamePropertyName))
            {
                valid = IsUserNameValid(viewModel.UserName);

                if(valid)
                {
                    IModelContainer<CredentialsModel> existingCredentials;

                    if (LookUpCredentialsId(viewModel.UserName, out existingCredentials))
                    {
                        _ignoreModelUpdates = true;
                        viewModel.Password = existingCredentials.Model.Password;
                        _passwordChanged = false;
                        _ignoreModelUpdates = false;
                    }
                }
            }
            else if (propertyName.Equals(EditCredentialsViewModel.PasswordPropertyName))
            {
                if(!_ignoreModelUpdates)
                {
                    //
                    // Password changed
                    //
                    _passwordChanged = true;
                }
            }

            return valid;
        }

        bool IEditCredentialsTask.Dismissing(IEditCredentialsViewModel viewModel, Action dismiss)
        {
            IModelContainer<CredentialsModel> existingCredentials;

            if (LookUpCredentialsId(viewModel.UserName, out existingCredentials))
            {
                _credentialsId = existingCredentials.Id;

                if (viewModel.SaveCredentials
                && (AreTrimmedStringsDifferent(viewModel.UserName, _credentials.Username) || _passwordChanged))
                {
                    //
                    // TODO: user wants to save the password, credentials object is in the data model, and
                    //       either the password or user name was changed, ask user if she wants to override
                    //       the password.
                    //
                }
            }
            else
            {
                _credentialsId = Guid.Empty;
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
                    _passwordChanged,
                    viewModel.SaveCredentials));
            }
        }

        void IEditCredentialsTask.Cancelled(IEditCredentialsViewModel viewModel)
        {
            if(null != this.Cancelled)
                this.Cancelled(this, EventArgs.Empty);
        }

        private bool LookUpCredentialsId(string userName, out IModelContainer<CredentialsModel> credentials)
        {
            bool found = false;

            userName = userName.EmptyIfNull().Trim();
            credentials = null;

            foreach(IModelContainer<CredentialsModel> container in _dataModel.LocalWorkspace.Credentials.Models)
            {
                if(userName.Equals(container.Model.Username.EmptyIfNull().Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    credentials = container;
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
