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
        private readonly ErrorMessageDelegate _okDelegate;
        private readonly ErrorMessageDelegate _cancelDelegate;

        public ErrorMessageArgs(IRdpError error, ErrorMessageDelegate okDelegate, ErrorMessageDelegate cancelDelegate)
        {
            _error = error;
            _okDelegate = okDelegate;
            _cancelDelegate = cancelDelegate;
        }

        public IRdpError Error { get { return _error; } }

        
        public ErrorMessageDelegate OkDelegate { get { return _okDelegate; } }

        
        public ErrorMessageDelegate CancelDelegate { get { return _cancelDelegate; } }
    }

    public class ErrorMessageViewModel : ViewModelBase
    {
        private readonly ICommand _okCommand;
        private readonly ICommand _cancelCommand;
        private ErrorMessageDelegate _okDelegate;
        private ErrorMessageDelegate _cancelDelegate;
        private IRdpError _error;

        public ErrorMessageViewModel()
        {
            _okCommand = new RelayCommand(new Action<object>(Ok));
            _cancelCommand = new RelayCommand(new Action<object>(Cancel));
        }

        public ICommand OkCommand { get { return _okCommand; } }

        
        public ICommand CancelCommand { get { return _cancelCommand; } }

        public IPresentableView DialogView { private get; set; }
        
        public IRdpError Error
        {
            get { return _error; }
            set
            {
                SetProperty(ref _error, value);
            }
        }
        
        public bool OkVisible { get { return _okDelegate != null;  } }
        
        public bool CancelVisible { get { return _cancelDelegate != null; } }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as ErrorMessageArgs);
            Contract.Assert(activationParameter is ErrorMessageArgs);
            ErrorMessageArgs args = (ErrorMessageArgs)activationParameter;

            this.Error = args.Error;

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
    }
}

