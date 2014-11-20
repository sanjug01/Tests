using RdClient.Shared.Navigation;
using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{

    public class DeleteDesktopsArgs
    {
        private readonly string _message;
        private readonly string _title;
        public string Message { get { return _message; } }
        public string Title { get { return _title; } }

        public DeleteDesktopsArgs(string message, , string title = "")
        {
            _message = message;
            _title = title;
        }
    }

    public class DeleteDesktopsViewModel : ViewModelBase
    {
        private readonly ICommand _deleteCommand;
        public ICommand DeleteCommand { get { return _deleteCommand; } }

        private readonly ICommand _cancelCommand;
        public ICommand CancelCommand { get { return _cancelCommand; } }

        public IPresentableView DialogView { private get; set; }

        private string _message;
        private string _title;
        public string Message
        {
            get { return _message; }
            set
            {
                SetProperty(ref _message, value, "Message");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value, "Title");
            }
        }

        private string _okString;
        public string OkString { get { return _okString; } }

        private string _cancelString;
        public string CancelString { get { return _cancelString; } }

        public bool OkVisible { get { return true;  } }

        public bool CancelVisible { get { return true; } }

        public DeleteDesktopsViewModel()
        {
            _deleteCommand = new RelayCommand(new Action<object>(DeleteDesktops));
            _cancelCommand = new RelayCommand(new Action<object>(Cancel));
        }

        private void DeleteDesktops(object o)
        {
            NavigationService.DismissModalView(DialogView);
            // TBD
        }

        private void Cancel(object o)
        {
            NavigationService.DismissModalView(DialogView);
            // TBD
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as DeleteDesktopsArgs);
            Contract.Assert(activationParameter is DeleteDesktopsArgs);
            DeleteDesktopsArgs args = (DeleteDesktopsArgs)activationParameter;

            this.Message = args.Message;
            this.Title = args.Title;

        }
    }
}

