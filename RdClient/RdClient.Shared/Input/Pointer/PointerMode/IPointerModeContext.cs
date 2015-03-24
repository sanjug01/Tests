using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Input.Pointer.PointerMode
{
    public interface IPointerModeContext
    {
        IPointerModeControl Control { get; }
        DoubleClickTimer Timer { get; }
        bool MoveThresholdExceeded(PointerEvent pointerEvent);
        int NumberOfContacts(PointerEvent pointerEvent);

        void TrackEvent(PointerEvent pointerEvent);
    }
}
