namespace RdClient.Shared.Input
{
    using RdClient.Shared.Helpers;
    using System.Diagnostics.Contracts;
    using Windows.System;
    using Windows.UI.Core;

    public class CoreWindowKeyboardCore : DisposableObject
    {
        private IKeyboardCaptureSink _sink;

        public CoreWindowKeyboardCore(IKeyboardCaptureSink sink)
        {
            Contract.Requires(null != sink);
            Contract.Ensures(null != _sink);

            _sink = sink;
        }

        protected override void DisposeManagedState()
        {
            base.DisposeManagedState();
            _sink = null;
        }

        public void ProcessKeyDown(VirtualKey virtualKey, CorePhysicalKeyStatus physicalKeyStatus)
        {
        }

        public void ProcessKeyUp(VirtualKey virtualKey, CorePhysicalKeyStatus physicalKeyStatus)
        {
        }

        public void ProcessCharacterReceived(uint keyCode, CorePhysicalKeyStatus physicalKeyStatus)
        {
        }
    }
}
