namespace RdClient.Shared.ViewModels.EditCredentialsTasks
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Diagnostics.Contracts;

    public sealed class InSessionCredentialsTask : EditCredentialsTaskBase
    {
        private readonly ISessionCredentials _sessionCredentials;
        private readonly ApplicationDataModel _dataModel;
        private readonly string _prompt;
        private readonly IValidationRule<string> _userNameRule;
        private readonly object _state;
        private IModelContainer<CredentialsModel> _savedCredentials;
        private bool _passwordChanged;

        public class ResultEventArgs : EventArgs
        {
            private readonly object _state;

            public object State { get { return _state; } }

            public ResultEventArgs(object state)
            {
                _state = state;
            }
        }

        public sealed class SubmittedEventArgs : ResultEventArgs
        {
            private readonly bool _saveCredentials;

            public bool SaveCredentials
            {
                get { return _saveCredentials; }
            }

            public SubmittedEventArgs(bool saveCredentials, object state) : base(state)
            {
                _saveCredentials = saveCredentials;
            }
        }

        public InSessionCredentialsTask(ISessionCredentials sessionCredentials,
            ApplicationDataModel dataModel,
            string prompt,
            object state)
        {
            Contract.Assert(null != sessionCredentials);
            Contract.Ensures(null != _sessionCredentials);
            Contract.Assert(null != dataModel);
            Contract.Ensures(null != _dataModel);

            _sessionCredentials = sessionCredentials;
            _dataModel = dataModel;
            _prompt = prompt;
            _state = state;
            _userNameRule = new UsernameFormatValidationRule();
            _savedCredentials = FindSavedCredentials(sessionCredentials.Credentials.Username);
            _passwordChanged = false;
        }

        public event EventHandler<SubmittedEventArgs> Submitted;
        public event EventHandler<ResultEventArgs> Cancelled;

        protected override void OnPresenting(IEditCredentialsViewModel viewModel)
        {
            viewModel.UserName = _sessionCredentials.Credentials.Username;
            viewModel.Password = _sessionCredentials.Credentials.Password;
            viewModel.CanSaveCredentials = true;
            viewModel.Prompt = _prompt;
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
                this.Submitted(this, new SubmittedEventArgs(viewModel.SaveCredentials, _state));
            }
        }

        protected override void OnCancelled(IEditCredentialsViewModel viewModel)
        {
            if (null != this.Cancelled)
                this.Cancelled(this, new ResultEventArgs(_state));
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
                    else if(!_passwordChanged)
                    {
                        //
                        // Clear the password from another saved credentials so user cannot try the saved password
                        // with any user name for that it was not saved.
                        //
                        viewModel.Password = null;
                        _passwordChanged = true;
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
            return _userNameRule.Validate(viewModel.UserName).Status == ValidationResultStatus.Valid;
        }

        private bool IsNewPasswordValid(IEditCredentialsViewModel viewModel)
        {
            return true;
        }

        private IModelContainer<CredentialsModel> FindSavedCredentials(string userName)
        {
            IModelContainer<CredentialsModel> container = null;

            foreach(IModelContainer<CredentialsModel> c in _dataModel.Credentials.Models)
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
