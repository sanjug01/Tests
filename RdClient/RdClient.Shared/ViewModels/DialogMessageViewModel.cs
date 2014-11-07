using RdClient.Navigation;
using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public delegate void DialogMessageDelegate();

    public class DialogMessageArgs
    {
        private readonly string _message;
        public string Message { get { return _message; } }

        private readonly DialogMessageDelegate _okDelegate;
        public DialogMessageDelegate OkDelegate { get { return _okDelegate; } }

        private readonly DialogMessageDelegate _cancelDelegate;
        public DialogMessageDelegate CancelDelegate { get { return _cancelDelegate; } }

        public DialogMessageArgs(string message, DialogMessageDelegate okDelegate, DialogMessageDelegate cancelDelegate)
        {
            _message = message;
            _okDelegate = okDelegate;
            _cancelDelegate = cancelDelegate;
        }
    }

    public class DialogMessageViewModel : ViewModelBase
    {
        private readonly ICommand _okCommand;
        public ICommand OkCommand { get { return _okCommand; } }

        private readonly ICommand _cancelCommand;
        public ICommand CancelCommand { get { return _cancelCommand; } }

        public INavigationService NavigationService { private get; set; }
        public IPresentableView DialogView { private get; set; }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                SetProperty(ref _message, value, "Message");
            }
        }

        private DialogMessageDelegate _okDelegate;
        public bool OkVisible { get { return _okDelegate != null;  } }

        private DialogMessageDelegate _cancelDelegate;
        public bool CancelVisible { get { return _cancelDelegate != null; } }

        public DialogMessageViewModel()
        {
            _okCommand = new RelayCommand(new Action<object>(Ok));
            _cancelCommand = new RelayCommand(new Action<object>(Cancel));
        }

        private void Ok(object o)
        {
            if(_okDelegate != null)
            {
                _okDelegate.Invoke();
            }

            NavigationService.DismissModalView(DialogView);
        }

        private void Cancel(object o)
        {
            if(_cancelDelegate != null)
            {
                _cancelDelegate.Invoke();
            }

            NavigationService.DismissModalView(DialogView);
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Contract.Requires(null != navigationService);
            Contract.Requires(null != activationParameter as DialogMessageArgs);

            NavigationService = navigationService;

            Message = (activationParameter as DialogMessageArgs).Message;

            _okDelegate = (activationParameter as DialogMessageArgs).OkDelegate;
            OnPropertyChanged("OkVisible");

            _cancelDelegate = (activationParameter as DialogMessageArgs).CancelDelegate;
            OnPropertyChanged("CancelVisible");
        }
    }
}

