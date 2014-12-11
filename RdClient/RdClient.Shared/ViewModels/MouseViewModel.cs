using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class MouseViewModel : MutableObject
    {
        private readonly ICommand _pointerMovedCommand;
        public ICommand PointerMovedCommand { get { return _pointerMovedCommand; } }

        private readonly ICommand _pointerPressedCommand;
        public ICommand PointerPressedCommand { get { return _pointerPressedCommand; } }

        private readonly ICommand _pointerReleasedCommand;
        public ICommand PointerReleasedCommand { get { return _pointerReleasedCommand; } }

        private readonly ICommand _pointerCanceledCommand;
        public ICommand PointerCanceledCommand { get { return _pointerCanceledCommand; } }

        private readonly ICommand _manipulationStartedCommand;
        public ICommand ManipulationStartedCommand { get { return _manipulationStartedCommand; } }

        private readonly ICommand _manipulationCompletedCommand;
        public ICommand ManipulationCompletedCommand { get { return _manipulationCompletedCommand; } }

        private readonly ICommand _manipulationInertiaStartingCommand;
        public ICommand ManipulationInertiaStartingCommand { get { return _manipulationInertiaStartingCommand; } }

        private readonly ICommand _manipulationDeltaCommand;
        public ICommand ManipulationDeltaCommand { get { return _manipulationDeltaCommand; } }
        

    }
}
