using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Navigation;
using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public delegate void ErrorMessageDelegate();

    public class ErrorMessageArgs
    {
        private readonly IRdpError _error;
        public IRdpError Error { get { return _error; } }

        private readonly string _title;
        public string Title { get { return _title; } }

        private readonly ErrorMessageDelegate _okDelegate;
        public ErrorMessageDelegate OkDelegate { get { return _okDelegate; } }

        private readonly ErrorMessageDelegate _cancelDelegate;
        public ErrorMessageDelegate CancelDelegate { get { return _cancelDelegate; } }

        private readonly string _okString;
        public string OkString { get { return _okString; } }

        private readonly string _cancelString;
        public string CancelString { get { return _cancelString; } }

        public ErrorMessageArgs(IRdpError error, ErrorMessageDelegate okDelegate, ErrorMessageDelegate cancelDelegate, string okString = "OK (d)", string cancelString = "Cancel (d)", string title = "")
        {
            _error = error;
            _okDelegate = okDelegate;
            _cancelDelegate = cancelDelegate;
            _okString = okString;
            _cancelString = cancelString;
            _title = title;
        }
    }

    public class ErrorMessageViewModel : ViewModelBase
    {
        private readonly ICommand _okCommand;
        public ICommand OkCommand { get { return _okCommand; } }

        private readonly ICommand _cancelCommand;
        public ICommand CancelCommand { get { return _cancelCommand; } }

        public IPresentableView DialogView { private get; set; }

        private IRdpError _message;
        public IRdpError Error
        {
            get { return _message; }
            set
            {
                SetProperty(ref _message, value);
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private string _okString;
        public string OkString { get { return _okString; } }

        private string _cancelString;
        public string CancelString { get { return _cancelString; } }

        private ErrorMessageDelegate _okDelegate;
        public bool OkVisible { get { return _okDelegate != null;  } }

        private ErrorMessageDelegate _cancelDelegate;
        public bool CancelVisible { get { return _cancelDelegate != null; } }

        public ErrorMessageViewModel()
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
            Contract.Requires(null != activationParameter as ErrorMessageArgs);
            Contract.Assert(activationParameter is ErrorMessageArgs);
            ErrorMessageArgs args = (ErrorMessageArgs)activationParameter;

            this.Error = args.Error;
            this.Title = args.Title;

            if (_okDelegate != args.OkDelegate)
            {
                _okDelegate = args.OkDelegate;
                EmitPropertyChanged("OkVisible");
            }

            if (_cancelDelegate != args.CancelDelegate)
            {
                _cancelDelegate = args.CancelDelegate;
                EmitPropertyChanged("CancelVisible");
            }

            _okString = args.OkString;
            _cancelString = args.CancelString;
        }
    }
}

