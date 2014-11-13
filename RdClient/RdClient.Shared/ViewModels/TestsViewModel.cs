using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace RdClient.Shared.ViewModels
{
    /// <summary>
    /// wrapper for activation parameters when presenting the TestsView
    /// </summary>
    public class TestsViewModelArgs
    {
        public TestsViewModelArgs(Desktop desktop, Credentials credentials)
        {
            this.Desktop = desktop;
            this.Credentials = credentials;
        }

        public Desktop Desktop { get; private set; }
        public Credentials Credentials { get; private set; }
    }

    /// <summary>
    /// view model to support integrated tests.
    ///      * these tests should be removed from shiped product.
    /// </summary>
    public sealed class TestsViewModel : ViewModelBase, IApplicationBarItemsSource
    {
        private readonly BarItemModel _separatorItem, _homeItem, _backItem, _forwardItem, _rightSeparatorItem;

        // manage desktops appbar items
        private readonly BarItemModel _addItem;
        private readonly BarItemModel _editItem;
        private readonly BarItemModel _deleteItem;

        // tests appbar items
        private readonly BarItemModel _testStressItem;
        private readonly BarItemModel _testConnectItem;
        private readonly BarItemModel _testDesktopsItem;

        private ConnectionInformation _connectionInformation;

        private readonly ObservableCollection<Desktop> _desktops;
        private readonly ObservableCollection<Credentials> _users;
        private IList<object> _selectedDesktops;

        private RelayCommand AddDesktopCommand { get; set; }
        private RelayCommand EditDesktopCommand { get; set; }
        private RelayCommand DeleteDesktopCommand { get; set; }
        private RelayCommand ConnectTestCommand { get; set; }
        private RelayCommand DesktopsTestCommand { get; set; }



        public ICommand StressTestCommand { get; private set; }
        public ICommand GoHomeCommand { get; private set; }

        public IRdpConnectionFactory RdpConnectionFactory { private get; set; }
        public ObservableCollection<Desktop> Desktops { get { return _desktops; } }
        public ObservableCollection<Credentials> Users { get { return _users; } }

        public IList<object> SelectedDesktops
        {
            private get { return _selectedDesktops; }
            set
            {
                SetProperty(ref _selectedDesktops, value, "SelectedDesktops");
                EditDesktopCommand.EmitCanExecuteChanged();
                DeleteDesktopCommand.EmitCanExecuteChanged();
            }
        }

        public TestsViewModel()
        {
            StressTestCommand = new RelayCommand(new Action<object>(StressTest));
            GoHomeCommand = new RelayCommand(new Action<object>(GoHome));

            AddDesktopCommand = new RelayCommand(o => this.AddDesktopCommandExecute(o), o => this.CanAddDesktopCommandExecute());
            EditDesktopCommand = new RelayCommand(o => this.EditDesktopCommandExecute(o), o => this.CanEditDesktopCommandExecute());
            DeleteDesktopCommand = new RelayCommand(o => this.DeleteDesktopCommandExecute(o), o => this.CanDeleteDesktopCommandExecute());

            ConnectTestCommand = new RelayCommand(new Action<object>(ConnectTests));
            DesktopsTestCommand = new RelayCommand(new Action<object>(DesktopTests));

            _separatorItem = new SeparatorBarItemModel();
            _homeItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Home, new RelayCommand(o => GoHome(o)), "Home");
            _backItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Back, new RelayCommand(o => _backItem.IsVisible = false), "Back");
            _forwardItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Forward, new RelayCommand(o => _backItem.IsVisible = true), "Forward");

            _addItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Add, AddDesktopCommand, "Add");
            _editItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Edit, EditDesktopCommand, "Edit");
            _deleteItem = new SegoeGlyphBarButtonModel(SegoeGlyph.Trash, DeleteDesktopCommand, "Delete",
                BarItemModel.ItemAlignment.Right);

            _rightSeparatorItem = new SeparatorBarItemModel(BarItemModel.ItemAlignment.Right);
            _testStressItem = new SegoeGlyphBarButtonModel(SegoeGlyph.People, StressTestCommand, "StressTest",
                BarItemModel.ItemAlignment.Right);
            _testConnectItem = new SegoeGlyphBarButtonModel(SegoeGlyph.People, ConnectTestCommand, "ConnectTests",
                BarItemModel.ItemAlignment.Right);
            _testDesktopsItem = new SegoeGlyphBarButtonModel(SegoeGlyph.People, DesktopsTestCommand, "DesktopsTests",
                BarItemModel.ItemAlignment.Right);

            _desktops = new ObservableCollection<Desktop>();
            _users = new ObservableCollection<Credentials>();
            _selectedDesktops = null;
            this.LoadTestData();
        }

        private void AddDesktopCommandExecute(object o)
        {
            // static credentials
            Credentials user = new Credentials() { Username = "tslabadmin", Domain = "", Password = "1234AbCd", HaveBeenPersisted = false };

            AddOrEditDesktopViewModelArgs args = new AddOrEditDesktopViewModelArgs(null, user, true,
                newDesktop => { this.Desktops.Add(newDesktop); });
            NavigationService.PushModalView("AddOrEditDesktopView", args);
        }

        private bool CanAddDesktopCommandExecute()
        {
            // can always add, unless we use a maximum number
            int maxAllowedDesktops = 20;
            if (null == this.Desktops)
            {
                return false;
            }
            return (maxAllowedDesktops > this.Desktops.Count);
        }

        private void EditDesktopCommandExecute(object o)
        {
            Contract.Requires(null == this.SelectedDesktops);

            if (this.SelectedDesktops.Count > 0) { 
                // static credentials
                Credentials user = new Credentials() { Username = "tslabadmin", Domain = "", Password = "1234AbCd", HaveBeenPersisted = false };
                Desktop desktop = this.SelectedDesktops[0] as Desktop;
                int desktopIndex = this.Desktops.IndexOf(desktop);

                // delegate is not necessary, since the selected desktop will be directly updated, but refreshing the list may be required
                AddOrEditDesktopViewModelArgs args = new AddOrEditDesktopViewModelArgs(desktop, user, false,
                    updatedDesktop => { this.Desktops[desktopIndex] = updatedDesktop; });

                NavigationService.PushModalView("AddOrEditDesktopView", args);
            }
        }

        private bool CanEditDesktopCommandExecute()
        {
            if (null == this.SelectedDesktops)
            {
                return false;
            }
            return (1 == this.SelectedDesktops.Count);
        }

        private void DeleteDesktopCommandExecute(object o)
        {
            Debug.WriteLine("Delete Desktop(s)!");
            this.NavigationService.PushModalView("DialogMessage", 
                new DialogMessageArgs("Delete desktops(d)",
                    () => { this.DeleteSelectedDesktops(); }, 
                    () => { },
                    "Delete(d)"));
        }

        private void DeleteSelectedDesktops()
        {
            int c = SelectedDesktops.Count;
            while (c > 0)
            {
                this.Desktops.Remove(SelectedDesktops[0] as Desktop);
                c--;
            }
        }

        private bool CanDeleteDesktopCommandExecute()
        {
            if (null == this.SelectedDesktops)
            {
                return false;
            }
            return (1 <= this.SelectedDesktops.Count);
        }

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as ConnectionInformation);
            _connectionInformation = activationParameter as ConnectionInformation;
        }

        private void StressTest(object o)
        {
            int iterations = 3;
            int i;
            AutoResetEvent are = new AutoResetEvent(false);
            SessionModel sm = new SessionModel(RdpConnectionFactory);
            IRdpConnection rdpConnection = null;

            EventHandler<ClientDisconnectedArgs> disconnectHandler = (s, a) => {
                if (a.DisconnectReason.Code != RdpDisconnectCode.UserInitiated)
                {
                    throw new Exception("unexpected disconnect");
                }
                else
                {
                    are.Set();
                }
            };
            EventHandler<FirstGraphicsUpdateArgs> connectHandler = (s, a) => { are.Set(); };

            sm.ConnectionCreated += (sender, args) =>
            {
                rdpConnection = args.RdpConnection;
                rdpConnection.Events.FirstGraphicsUpdate += connectHandler;
                rdpConnection.Events.ClientDisconnected += disconnectHandler;
            };

            for(i = 0; i < iterations; i++)
            {
                sm.Connect(_connectionInformation);
                
                are.WaitOne();

                sm.Disconnect();
                
                are.WaitOne();
            }
        }

        /// <summary>
        /// This test verifies the sequence Add desktop, connect/disconnect, delete desktop
        /// </summary>
        /// <param name="o">test parameter</param>
        private void ConnectTests(object o)
        {
            Debug.WriteLine("Running connect tests  ........");

            // TBD

            Debug.WriteLine("Running connect tests  ........ COMPLETED , without errors");
        }

        /// <summary>
        /// This test verifies Add/Edit/Delete desktops functionality.
        /// </summary>
        /// <param name="o"> test parameter</param>
        private void DesktopTests(object o)
        {
            Debug.WriteLine("Running desktop management tests   .......");
            bool delegateCalled = false;
            string returnHostName="some_host";
            string expectedHostName;
            AddOrEditDesktopViewModel vm = new AddOrEditDesktopViewModel();

            Desktop testDesktop = new Desktop() { HostName = "test_host" };
            Credentials testUser = new Credentials() { Username = "test_user", Domain = "", Password = "test_password", HaveBeenPersisted = false };

            // Test 1: add desktop, cancel
            AddOrEditDesktopViewModelArgs args = new AddOrEditDesktopViewModelArgs(null, testUser, true,
                newDesktop => { delegateCalled = true; });

            vm.Presenting(null, args);
            vm.Host = "unsaved_new_host";
            vm.CancelCommand.Execute(null);           
            RdTrace.IfCondThrow(delegateCalled, "Add desktop and cancel should not call saveDelegate!");


            // Test 2: add desktop, save
            delegateCalled = false;
            args = new AddOrEditDesktopViewModelArgs(null, testUser, true,
                newDesktop => { delegateCalled = true; returnHostName = newDesktop.HostName; });

            vm.Presenting(null, args);
            vm.Host = "saved_new_host";
            expectedHostName = vm.Host;
            vm.SaveCommand.Execute(null);
            RdTrace.IfCondThrow(!delegateCalled, "Add desktop and save should call saveDelegate!");
            RdTrace.IfCondThrow(returnHostName != expectedHostName, "Add user and save should save host name!");


            // Test 3: edit desktop, cancel
            delegateCalled = false;
            args = new AddOrEditDesktopViewModelArgs(testDesktop, testUser, false,
                newDesktop => { delegateCalled = true; });

            vm.Presenting(null, args);
            vm.Host = "not_updated_host";
            expectedHostName = testDesktop.HostName;
            vm.CancelCommand.Execute(null);
            RdTrace.IfCondThrow(delegateCalled, "Edit desktop and cancel should not call saveDelegate!");
            RdTrace.IfCondThrow(testDesktop.HostName != expectedHostName, "Edit desktop and cancel should not change host name!");

            // Test 4: edit desktop, save
            delegateCalled = false;
            args = new AddOrEditDesktopViewModelArgs(testDesktop, testUser, false,
                newDesktop => { delegateCalled = true; returnHostName = newDesktop.HostName; });

            vm.Presenting(null, args);
            vm.Host = "updated_host";
            expectedHostName = vm.Host;
            vm.SaveCommand.Execute(null);
            RdTrace.IfCondThrow(!delegateCalled, "Edit desktop and save should call saveDelegate!");
            RdTrace.IfCondThrow(returnHostName != expectedHostName, "Edit desktop and save should save host name!");
            RdTrace.IfCondThrow(testDesktop.HostName != expectedHostName, "Edit desktop and save should save update desktop!");

            Debug.WriteLine("Running desktop management tests   ....... COMPLETED , without errors");
        }

        private void GoHome(object o)
        {
            NavigationService.NavigateToView("view1", null);
        }

        IEnumerable<BarItemModel> IApplicationBarItemsSource.GetItems(IApplicationBarSite applicationBarSite)
        {
            return new BarItemModel[]
            {
                _homeItem,
                _separatorItem,
                _backItem,
                _forwardItem,
                _separatorItem,
                _addItem,
                _editItem,
                _deleteItem,
                _rightSeparatorItem,
                _testStressItem,
                _testConnectItem,
                _testDesktopsItem
            };
        }

        private void LoadTestData()
        {
            Contract.Requires(null != _desktops);
            Contract.Requires(null != _users);

            for(int i = 0; i < 10; i++ )
            {
                Desktop desktop = new Desktop() { HostName = "testhost" + i };
                _desktops.Add(desktop);

                Credentials user = new Credentials() { Username = "testuser" + i, Domain = "TestDomain.com", Password = "1234AbCd", HaveBeenPersisted = false };
                _users.Add(user);

            }

            AddDesktopCommand.EmitCanExecuteChanged();
        }
    }
}
