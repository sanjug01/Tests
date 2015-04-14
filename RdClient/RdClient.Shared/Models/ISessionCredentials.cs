namespace RdClient.Shared.Models
{
    using RdClient.Shared.Data;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Information about credentials model used by a particular session, 
    /// either as connection or gateway credentials
    /// </summary>
    public interface ISessionCredentials
    {
        CredentialsModel Credentials { get; }


        void ApplySavedCredentials(IModelContainer<CredentialsModel> savedCredentials);

        void SetChangedPassword(string changedPassword);

     }
}
