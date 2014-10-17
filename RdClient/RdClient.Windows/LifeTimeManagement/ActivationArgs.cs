using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.LifeTimeManagement
{
    public class ActivationArgs : IActivationArgs
    {
        public string Arguments
        { get; set; }

        public string TileId
        { get; set; }

        public Windows.ApplicationModel.Activation.ActivationKind Kind
        { get; set; }

        public Windows.ApplicationModel.Activation.ApplicationExecutionState PreviousExecutionState
        { get; set; }

        public Windows.ApplicationModel.Activation.SplashScreen SplashScreen
        { get; set; }

        public int CurrentlyShownApplicationViewId
        { get; set; }

        public bool PrelaunchActivated
        { get; set; }
    }
}
