using RdClient.Shared.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class ConnectionCenterViewModel : ViewModelBase
    {
        private readonly RelayCommand _addDesktopCommand;
        private ObservableCollection<DesktopViewModel> _desktopViewModels;

        public ConnectionCenterViewModel()
        {
            _addDesktopCommand = new RelayCommand(AddDesktopExecute);
            this.PropertyChanged += ConnectionCenterViewModel_PropertyChanged;
        }

        public ObservableCollection<DesktopViewModel> DesktopViewModels
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
            foreach (DesktopViewModel vm in this.DesktopViewModels)
            {
                vm.Presenting(this.NavigationService, null);
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
                ObservableCollection<DesktopViewModel> desktopVMs = new ObservableCollection<DesktopViewModel>();
                foreach (Desktop desktop in this.DataModel.Desktops)
                {
                    DesktopViewModel vm = new DesktopViewModel(desktop);
                    vm.DataModel = this.DataModel;
                    vm.Presenting(this.NavigationService, desktop);                                       
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
                    vm.Presenting(this.NavigationService, null);
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
