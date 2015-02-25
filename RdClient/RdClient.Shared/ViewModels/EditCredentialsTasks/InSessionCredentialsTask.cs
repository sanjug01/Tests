﻿namespace RdClient.Shared.ViewModels.EditCredentialsTasks
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    public sealed class InSessionCredentialsTask : EditCredentialsTaskBase
    {
        private readonly SessionCredentials _sessionCredentials;
        private readonly ApplicationDataModel _dataModel;
        private readonly IValidationRule _userNameRule;
        private IModelContainer<CredentialsModel> _savedCredentials;
        private bool _passwordChanged;

        public class SubmittedEventArgs : EventArgs
        {
            private readonly bool _saveCredentials;

            public bool SaveCredentials
            {
                get { return _saveCredentials; }
            }

            public SubmittedEventArgs(bool saveCredentials)
            {
                _saveCredentials = saveCredentials;
            }
        }

        public InSessionCredentialsTask(SessionCredentials sessionCredentials, ApplicationDataModel dataModel)
        {
            Contract.Assert(null != sessionCredentials);
            Contract.Ensures(null != _sessionCredentials);
            Contract.Assert(null != dataModel);
            Contract.Ensures(null != _dataModel);

            _sessionCredentials = sessionCredentials;
            _dataModel = dataModel;
            _userNameRule = new UsernameValidationRule();
            _savedCredentials = FindSavedCredentials(sessionCredentials.Credentials.Username);
            _passwordChanged = false;
        }

        public event EventHandler<SubmittedEventArgs> Submitted;
        public event EventHandler Cancelled;

        protected override void OnPresenting(IEditCredentialsViewModel viewModel)
        {
            viewModel.UserName = _sessionCredentials.Credentials.Username;
            viewModel.Password = _sessionCredentials.Credentials.Password;
            viewModel.CanSaveCredentials = true;
            viewModel.DismissLabel = "d:Connect";
        }

        protected override void OnDismissing(IEditCredentialsViewModel viewModel, IEditCredentialsViewControl viewControl)
        {
            if (viewModel.SaveCredentials && _passwordChanged && null != _savedCredentials)
            {
                viewControl.AskConfirmation(EditCredentialsConfirmation.OverridePassword);
            }
            else
            {
                viewControl.Submit();
            }
        }

        protected override void OnDismissed(IEditCredentialsViewModel viewModel)
        {
            if(null != _savedCredentials)
            {
                _sessionCredentials.ApplySavedCredentials(_savedCredentials);

                if (_passwordChanged)
                    _sessionCredentials.SetChangedPassword(viewModel.Password);
            }
            else
            {
                _sessionCredentials.Credentials.Username = viewModel.UserName;
                _sessionCredentials.Credentials.Password = viewModel.Password;
            }
            //
            // Emit event to let subscribers know that the credentials have been submitted.
            //
            if(null != this.Submitted)
            {
                this.Submitted(this, new SubmittedEventArgs(viewModel.SaveCredentials));
            }
        }

        protected override void OnCancelled(IEditCredentialsViewModel viewModel)
        {
            if (null != this.Cancelled)
                this.Cancelled(this, EventArgs.Empty);
        }

        protected override bool ValidateChangedProperty(IEditCredentialsViewModel viewModel, string propertyName)
        {
            bool valid = base.ValidateChangedProperty(viewModel, propertyName);

            if(valid)
            {
                if(EditCredentialsViewModel.UserNamePropertyName == propertyName)
                {
                    _savedCredentials = FindSavedCredentials(viewModel.UserName);

                    if(null != _savedCredentials)
                    {
                        viewModel.Password = _savedCredentials.Model.Password;
                        _passwordChanged = false;
                    }

                    valid = IsNewUserNameValid(viewModel);
                }
                else if(EditCredentialsViewModel.PasswordPropertyName == propertyName)
                {
                    valid = IsNewPasswordValid(viewModel);
                    _passwordChanged = true;
                }
            }

            return valid;
        }

        protected override bool Validate(IEditCredentialsViewModel viewModel)
        {
            bool valid = base.Validate(viewModel);

            if(valid)
            {
                valid = IsNewUserNameValid(viewModel) && IsNewPasswordValid(viewModel);
            }

            return valid;
        }

        private bool IsNewUserNameValid(IEditCredentialsViewModel viewModel)
        {
            bool valid = !string.IsNullOrWhiteSpace(viewModel.UserName);

            if(valid)
            {
                valid = _userNameRule.Validate(viewModel.UserName, CultureInfo.CurrentUICulture);
            }

            return valid;
        }

        private bool IsNewPasswordValid(IEditCredentialsViewModel viewModel)
        {
            return true;
        }

        private IModelContainer<CredentialsModel> FindSavedCredentials(string userName)
        {
            IModelContainer<CredentialsModel> container = null;

            foreach(IModelContainer<CredentialsModel> c in _dataModel.LocalWorkspace.Credentials.Models)
            {
                if( string.Equals(c.Model.Username, userName, StringComparison.OrdinalIgnoreCase) )
                {
                    container = c;
                    break;
                }
            }

            return container;
        }
    }
}
