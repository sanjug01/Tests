using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public interface IPointerModeControl
    {
        event EventHandler<EventArgs> GestureBegan;
        event EventHandler<EventArgs> GestureEnded;
    }
}
