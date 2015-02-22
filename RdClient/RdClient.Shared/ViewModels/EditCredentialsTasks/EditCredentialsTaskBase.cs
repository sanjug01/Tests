namespace RdClient.Shared.ViewModels.EditCredentialsTasks
{
    using System.Diagnostics.Contracts;

    public abstract class EditCredentialsTaskBase : IEditCredentialsTask
    {
        protected sealed class Token
        {
            private readonly IEditCredentialsViewModel _viewModel;
            private readonly IEditCredentialsViewControl _viewControl;

            public static Token FromObject(object token)
            {
                Contract.Assert(token is Token);
                return (Token)token;
            }

            public Token(IEditCredentialsViewModel viewModel, IEditCredentialsViewControl viewControl)
            {
                Contract.Requires(null != viewModel);
                Contract.Requires(null != viewControl);
                _viewModel = viewModel;
                _viewControl = viewControl;
            }

            public IEditCredentialsViewModel ViewModel { get { return _viewModel; } }
            public IEditCredentialsViewControl ViewControl { get { return _viewControl; } }
        }

        object IEditCredentialsTask.Presenting(IEditCredentialsViewModel viewModel, IEditCredentialsViewControl control)
        {
            return new Token(viewModel, control);
        }

        void IEditCredentialsTask.Populate(object token)
        {
            OnPresenting(Token.FromObject(token).ViewModel);
        }

        bool IEditCredentialsTask.ValidateChangedProperty(object token, string propertyName)
        {
            return ValidateChangedProperty(Token.FromObject(token).ViewModel, propertyName);
        }

        bool IEditCredentialsTask.Validate(object token)
        {
            return Validate(Token.FromObject(token).ViewModel);
        }

        void IEditCredentialsTask.Dismissing(object token)
        {
            Token t = Token.FromObject(token);
            OnDismissing(t.ViewModel, t.ViewControl);
        }

        void IEditCredentialsTask.Dismissed(object token)
        {
            OnDismissed(Token.FromObject(token).ViewModel);
        }

        void IEditCredentialsTask.Cancelled(object token)
        {
            OnCancelled(Token.FromObject(token).ViewModel);
        }

        protected virtual void OnPresenting(IEditCredentialsViewModel viewModel)
        {
        }

        protected virtual bool ValidateChangedProperty(IEditCredentialsViewModel viewModel, string propertyName)
        {
            return true;
        }

        protected virtual bool Validate(IEditCredentialsViewModel viewModel)
        {
            return true;
        }

        protected virtual void OnDismissing(IEditCredentialsViewModel viewModel, IEditCredentialsViewControl viewControl)
        {
            viewControl.Dismiss();
        }

        protected virtual void OnDismissed(IEditCredentialsViewModel viewModel)
        {
        }

        protected virtual void OnCancelled(IEditCredentialsViewModel viewModel)
        {
        }
    }
}
