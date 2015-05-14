namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System;

    public enum UserComboBoxType
    {
        AddNew = 0,
        AskEveryTime = 1,
        Credentials = 2
    }

    public sealed class UserComboBoxOrder : IComparer<UserComboBoxElement>
    {
        public int Compare(UserComboBoxElement x, UserComboBoxElement y)
        {
            Contract.Assert(null != x);
            Contract.Assert(null != y);

            int compareType = x.UserComboBoxType < y.UserComboBoxType ? -1 : x.UserComboBoxType > y.UserComboBoxType ? 1 : 0;

            if (0 == compareType && UserComboBoxType.Credentials == x.UserComboBoxType)
            {
                // compare by user name if both have credentials
                Contract.Assert(null != x.Credentials && null != x.Credentials.Model);
                Contract.Assert(null != y.Credentials && null != x.Credentials.Model);

                return string.Compare(x.Credentials.Model.Username, y.Credentials.Model.Username);
            }

            return compareType;    
        }
    }


    public class UserComboBoxElement
    {
        private readonly IModelContainer<CredentialsModel> _credentials;

        public IModelContainer<CredentialsModel> Credentials { get { return _credentials; } }

        private readonly UserComboBoxType _userComboBoxType;
        public UserComboBoxType UserComboBoxType { get { return _userComboBoxType; } }

        public UserComboBoxElement(UserComboBoxType userComboBoxType, IModelContainer<CredentialsModel> credentials = null)
        {            
            _userComboBoxType  = userComboBoxType;
            _credentials = credentials;
        }
    }
}
