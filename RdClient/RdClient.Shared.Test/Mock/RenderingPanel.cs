﻿using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models;
using RdClient.Shared.Models.Viewport;
using RdMock;
using System;

namespace RdClient.Shared.Test.Mock
{
    public class RenderingPanel : MockBase, IRenderingPanel
    {
        public event EventHandler Ready;

        public event EventHandler<IPointerEventBase> PointerChanged;

        public IViewport Viewport
        {
            get;
            set;
        }

        public void ChangeMouseVisibility(bool visibility)
        {
            Invoke(new object[] { visibility });
        }

        public void ChangeMouseCursorShape(Shared.Input.Pointer.MouseCursorShape shape)
        {
            Invoke(new object[] { shape });
        }

        public void MoveMouseCursor(Windows.Foundation.Point point)
        {
            Invoke(new object[] { point });
        }

        public void EmitRead()
        {
            Ready(this, EventArgs.Empty);
        }
    }
}
