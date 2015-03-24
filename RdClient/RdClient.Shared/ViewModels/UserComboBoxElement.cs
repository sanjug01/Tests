namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;

    public enum UserComboBoxType
    {
        Credentials,
        AskEveryTime,
        AddNew
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
