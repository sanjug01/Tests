using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using System;

namespace RdClient.Shared.Input.Pointer
{
    public interface IPointerCapture
    {
        void OnPointerChanged(object sender, PointerEventArgs args);
        IRemoteSessionControl RemoteSessionControl { set; }
        IExecutionDeferrer ExecutionDeferrer { set; }
        IRenderingPanel RenderingPanel { set; }
    }
}
