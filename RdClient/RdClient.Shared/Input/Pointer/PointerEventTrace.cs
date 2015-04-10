using RdClient.Shared.CxWrappers;
using System.Diagnostics.Contracts;

namespace RdClient.Shared.Input.Pointer
{
    /// <summary>
    /// traces the last 2 pointer events for a pointer
    /// </summary>
    public class PointerEventTrace
    {
        public PointerEventOld LastEvent { private set; get; }
        public PointerEventOld PreviousEvent { private set; get; } 

        public void SetLastEvent(PointerEventOld pointerEvent)
        {
            PreviousEvent = LastEvent;
            LastEvent = pointerEvent;
        }

        public double DeltaX
        {
            get
            {
                if(null == PreviousEvent || null == LastEvent)
                {
                    return 0;
                }
                else
                {
                    return LastEvent.Position.X - PreviousEvent.Position.X;
                }
            }
        }

        public double DeltaY
        {
            get
            {
                if(null == PreviousEvent)
                {
                    return 0;
                }
                else
                {
                    return LastEvent.Position.Y - PreviousEvent.Position.Y;
                }
            }
        }

        /// <summary>
        /// deltaX relative to a newer event
        /// </summary>
        /// <param name="newEvent"> a newer event</param>
        /// <returns>delta X</returns>
        public double DeltaXTo(PointerEventOld newEvent)
        {
            return newEvent.Position.X - LastEvent.Position.X;
        }

        /// <summary>
        /// deltaY relative to a newer event
        /// </summary>
        /// <param name="newEvent"> a newer event</param>
        /// <returns>delta Y</returns>
        public double DeltaYTo(PointerEventOld newEvent)
        {
            return newEvent.Position.Y - LastEvent.Position.Y;
        }

        public bool IsUpdated
        {
            get { return null != PreviousEvent; }
        }

        public PointerEventTrace(PointerEventOld pointerEvent)
        {
            Contract.Requires(null != pointerEvent);
            this.LastEvent = pointerEvent;
            this.PreviousEvent = null;
        }

        public PointerEventTrace(PointerEventOld pointerEvent, PointerEventOld previousPointerEvent)
        {
            Contract.Requires(null != pointerEvent);
            this.LastEvent = pointerEvent;
            this.PreviousEvent = previousPointerEvent;
        }
    }
}
