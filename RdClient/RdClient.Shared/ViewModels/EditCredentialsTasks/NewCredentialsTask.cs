﻿namespace RdClient.Shared.ViewModels.EditCredentialsTasks
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ValidationRules;
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// Edit credentials task launched to add new credentials to the data model.
    /// Validation only checks that the user name is non-empty, valid and unique.
    /// </summary>
    public sealed class NewCredentialsTask : IEditCredentialsTask
    {
        private readonly IValidationRule _hostNameRule;
        private readonly ApplicationDataModel _dataModel;
        private readonly Action<Guid> _credentialsAdded;
        private readonly string _resourceName;
        private readonly CredentialsModel _credentials;

        public static void Present(INavigationService navigationService, ApplicationDataModel dataModel,
            string resourceName,
            Action<Guid> credentialsAdded)
        {
            navigationService.PushModalView("EditCredentialsView", new NewCredentialsTask(resourceName, dataModel, credentialsAdded));
        }

        private NewCredentialsTask(string resourceName, ApplicationDataModel dataModel, Action<Guid> credentialsAdded)
        {
            Contract.Requires(null != dataModel);
            Contract.Requires(null != _credentialsAdded);
            Contract.Ensures(null != _credentials);

            _hostNameRule = new HostNameValidationRule();
            _resourceName = resourceName;
            _dataModel = dataModel;
            _credentialsAdded = credentialsAdded;
            _credentials = new CredentialsModel();
        }

        void IEditCredentialsTask.Populate(IEditCredentialsViewModel viewModel)
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
            viewModel.DismissLabel = "d:Add Credentials";
        }

        bool IEditCredentialsTask.Validate(IEditCredentialsViewModel viewModel)
        {
            bool valid = true;

            string userName = TrimViewModelString(viewModel.UserName);

            if (string.IsNullOrEmpty(userName))
            {
                //
                // TODO: update the view model to show the "empty user name" error
                //
                valid = false;
            }
            else if(!_hostNameRule.Validate(userName, CultureInfo.CurrentCulture))
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
                foreach (IModelContainer<CredentialsModel> container in _dataModel.LocalWorkspace.Credentials.Models)
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

        void IEditCredentialsTask.Dismissed(IEditCredentialsViewModel viewModel)
        {
            //
            // Copy view model values to the model and save the model in the data model.
            //
            _credentials.Username = TrimViewModelString(viewModel.UserName);
            _credentials.Password = viewModel.Password;
            //
            // Add the model to the data model collection and call the delegate.
            //
            _credentialsAdded(_dataModel.LocalWorkspace.Credentials.AddNewModel(_credentials));
        }

        void IEditCredentialsTask.Cancelled(IEditCredentialsViewModel viewModel)
        {
        }

        private static string TrimViewModelString(string str)
        {
            return str.EmptyIfNull().Trim();
        }
    }
}