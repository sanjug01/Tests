namespace RdClient.Shared.ViewModels.EditCredentialsTasks
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Edit credentials task launched to add new credentials to the data model.
    /// Validation only checks that the user name is non-empty, valid and unique.
    /// </summary>
    public sealed class NewCredentialsTask : EditCredentialsTaskBase
    {
        private readonly IValidationRule<string> _userNameRule;
        private readonly ApplicationDataModel _dataModel;
        private readonly Action<Guid> _credentialsAdded;
        private readonly Action _viewCancelled;
        private readonly string _resourceName;
        private readonly CredentialsModel _credentials;

        public static void Present(string viewName, INavigationService navigationService, ApplicationDataModel dataModel,
            string resourceName,
            Action<Guid> credentialsAdded,
            Action viewCancelled = null)
        {
            navigationService.PushModalView(viewName, new NewCredentialsTask(resourceName, dataModel, credentialsAdded, viewCancelled));
        }

        private NewCredentialsTask(string resourceName, ApplicationDataModel dataModel,
            Action<Guid> credentialsAdded,
            Action viewCancelled)
        {
            Contract.Requires(null != dataModel);
            Contract.Requires(null != _credentialsAdded);
            Contract.Ensures(null != _credentials);

            _userNameRule = new UsernameFormatValidationRule();
            _resourceName = resourceName;
            _dataModel = dataModel;
            _credentialsAdded = credentialsAdded;
            _viewCancelled = viewCancelled;
            _credentials = new CredentialsModel();
        }

        protected override void OnPresenting(IEditCredentialsViewModel viewModel)
        {
            //
            // Copy model data to the view model even though the model has only default values
            //
            viewModel.ResourceName = _resourceName;
            viewModel.UserName = _credentials.Username;
            viewModel.Password = _credentials.Password;
            //
            // Do not show the "save credentials" check-box because the credentials
            // will be saved unconditionally.
            //
            viewModel.CanSaveCredentials = false;
        }

        protected override bool Validate(IEditCredentialsViewModel viewModel)
        {
            bool valid = true;

            string userName = TrimViewModelString(viewModel.UserName);

            if(_userNameRule.Validate(userName).Status != ValidationResultStatus.Valid)
            {
                //
                // TODO: update the view model to show the "invalid user name" error
                //
                valid = false;
            }
            else
            {
                //
                // Do not permit duplicate user name
                //
                foreach (IModelContainer<CredentialsModel> container in _dataModel.Credentials.Models)
                {
                    if(string.Equals(container.Model.Username, userName,StringComparison.OrdinalIgnoreCase))
                    {
                        //
                        // TODO: update the view model to show the "duplicate user name" error
                        //
                        valid = false;
                        break;
                    }
                }
            }

            return valid;
        }

        protected override void OnDismissed(IEditCredentialsViewModel viewModel)
        {
            //
            // Copy view model values to the model and save the model in the data model.
            //
            _credentials.Username = TrimViewModelString(viewModel.UserName);
            _credentials.Password = viewModel.Password;
            //
            // Add the model to the data model collection and call the delegate.
            //
            _credentialsAdded(_dataModel.Credentials.AddNewModel(_credentials));
        }

        protected override void OnCancelled(IEditCredentialsViewModel viewModel)
        {
            if (null != _viewCancelled)
                _viewCancelled();
        }

        private static string TrimViewModelString(string str)
        {
            return str.EmptyIfNull().Trim();
        }
    }
}
