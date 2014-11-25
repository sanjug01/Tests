using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class ConnectionCenterViewModel : ViewModelBase, IConnectionCenterViewModel
    {
        private readonly RelayCommand _addDesktopCommand;
        private ObservableCollection<IDesktopViewModel> _desktopViewModels;

        public ConnectionCenterViewModel()
        {
            _addDesktopCommand = new RelayCommand(AddDesktopExecute);
            this.PropertyChanged += ConnectionCenterViewModel_PropertyChanged;
        }

        public ObservableCollection<IDesktopViewModel> DesktopViewModels
        {
            get { return _desktopViewModels; }
            private set 
            {                
                SetProperty(ref _desktopViewModels, value);
            }
        }

        public ICommand AddDesktopCommand
        {
            get { return _addDesktopCommand; }
        }

        public bool HasDesktops
        {
            get { return this.DataModel.Desktops.Count > 0; }
        }

        protected override void OnPresenting(object activationParameter)
        {                        
            foreach (IViewModel vm in this.DesktopViewModels)
            {
                vm.Presenting(this.NavigationService, null, null);
            }
        }

        private void AddDesktopExecute(object o)
        {
            NavigationService.PushModalView("AddOrEditDesktopView", new AddOrEditDesktopViewModelArgs(null, null, true));
        }

        private void ConnectionCenterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DataModel")
            {
                ObservableCollection<IDesktopViewModel> desktopVMs = new ObservableCollection<IDesktopViewModel>();
                foreach (Desktop desktop in this.DataModel.Desktops)
                {
                    DesktopViewModel vm = new DesktopViewModel(desktop);
                    vm.DataModel = this.DataModel;
                    //
                    // TODO:    REFACTOR THIS! View models in collections do not participate in nav service activities
                    //          and should not rely on the IViewModel interface.
                    //          In this case, the desktop view models are not being presented.
                    //
                    ((IViewModel)vm).Presenting(this.NavigationService, desktop, null);                                       
                    desktopVMs.Add(vm);
                }
                this.DesktopViewModels = desktopVMs;
                this.DataModel.Desktops.CollectionChanged += Desktops_CollectionChanged;
                this.EmitPropertyChanged("HasDesktops");
                ((INotifyPropertyChanged)this.DesktopViewModels).PropertyChanged += DesktopViewModels_PropertyChanged;
            }
        }

        private void DesktopViewModels_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Count"))
            {
                this.EmitPropertyChanged("HasDesktops");
            }
        }

        private void Desktops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Desktop desktop in e.NewItems)
                {
                    DesktopViewModel vm = new DesktopViewModel(desktop);
                    vm.DataModel = this.DataModel;
                    //
                    // TODO:    REFACTOR THIS! View models in collections do not participate in nav service activities
                    //          and should not rely on the IViewModel interface.
                    //          In this case, the desktop view models are not being presented.
                    //
                    ((IViewModel)vm).Presenting(this.NavigationService, null, null);
                    this.DesktopViewModels.Add(vm);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var vmsToRemove = this.DesktopViewModels.Where(vm => e.OldItems.Contains(vm.Desktop)).ToList();
                foreach (DesktopViewModel vm in vmsToRemove)
                {
                    this.DesktopViewModels.Remove(vm);
                }
            }
        }
    }
}
