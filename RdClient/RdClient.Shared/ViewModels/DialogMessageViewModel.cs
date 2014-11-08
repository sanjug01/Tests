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

        private readonly string _okString;
        public string OkString { get { return _okString; } }

        private readonly string _cancelString;
        public string CancelString { get { return _cancelString; } }

        public DialogMessageArgs(string message, DialogMessageDelegate okDelegate, DialogMessageDelegate cancelDelegate, string okString = "OK (d)", string cancelString = "Cancel (d)")
        {
            _message = message;
            _okDelegate = okDelegate;
            _cancelDelegate = cancelDelegate;
            _okString = okString;
            _cancelString = cancelString;
        }
    }

    public class DialogMessageViewModel : ViewModelBase
    {
        private readonly ICommand _okCommand;
        public ICommand OkCommand { get { return _okCommand; } }

        private readonly ICommand _cancelCommand;
        public ICommand CancelCommand { get { return _cancelCommand; } }

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

        private string _okString;
        public string OkString { get { return _okString; } }

        private string _cancelString;
        public string CancelString { get { return _cancelString; } }

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
            NavigationService.DismissModalView(DialogView);

            if(_okDelegate != null)
            {
                _okDelegate();
            }
        }

        private void Cancel(object o)
        {
            NavigationService.DismissModalView(DialogView);

            if(_cancelDelegate != null)
            {
                _cancelDelegate();
            }
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as DialogMessageArgs);
            DialogMessageArgs args = activationParameter as DialogMessageArgs;

            Message = args.Message;

            _okDelegate = args.OkDelegate;
            OnPropertyChanged("OkVisible");

            _cancelDelegate = args.CancelDelegate;
            OnPropertyChanged("CancelVisible");

            _okString = args.OkString;
            _cancelString = args.CancelString;
        }
    }
}

