namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Input;
    using Windows.ApplicationModel;
    using Windows.UI.Xaml;

    public sealed class RichTextViewModel : AccessoryViewModelBase
    {
        private readonly RelayCommand _closeCommand;

        public RichTextViewModel()
        {
            _closeCommand = new RelayCommand(o => { this.DismissModal(null); });
        }

        public ICommand Close { get { return _closeCommand; } }

        protected override void DefaultAction()
        {
            DismissModal(null);
        }

    }
}
