namespace RdClient.Shared.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.UI.Core;

    public class BackRequestedEventArgsWrapper : IBackCommandArgs
    {
        private BackRequestedEventArgs _backEventArgs;
        public BackRequestedEventArgsWrapper(BackRequestedEventArgs backEventArgs)
        {
            _backEventArgs = backEventArgs;
        }

        public bool Handled
        {
            get
            {
                return _backEventArgs.Handled;
            }

            set
            {
                _backEventArgs.Handled = value;
            }
        }
    }
}
