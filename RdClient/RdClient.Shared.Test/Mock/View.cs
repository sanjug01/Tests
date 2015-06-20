using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.Foundation;
using RdMock;
using RdClient.Shared.Models.Viewport;
using RdClient.Shared.Input.Pointer;

namespace RdClient.Shared.Test.Mock
{
    public class View : MockBase, IPresentableView, IRemoteSessionView
    {
        private EventHandler<IPointerEventBase> _pointerChanged;

        public Size RenderingPanelSize { get; set; }

        public IViewModel ViewModel { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public IViewportTransform Transform
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler Closed;
        public void EmitClosedEvent()
        {
            if(Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<IPointerEventBase> PointerChanged
        {
            add { _pointerChanged += value; }
            remove { _pointerChanged -= value; }
        }

        public void EmitPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public IRenderingPanel ActivateNewRenderingPanel()
        {
            return (IRenderingPanel)Invoke(new object[] { });
        }

        public void Activating(object activationParameter)
        {
            Invoke(new object[] { activationParameter });
        }

        public void Dismissing()
        {
            Invoke(new object[] { });
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Invoke(new object[] { navigationService, activationParameter });
        }

        public void RecycleRenderingPanel(IRenderingPanel renderingPanel)
        {
            Invoke(new object[] { renderingPanel });
        }
    }
}
