using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Windows.Input;

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
        private readonly BarItemModel _testDisconnectItem;
        private readonly BarItemModel _testDesktopsItem;

        private ConnectionInformation _connectionInformation;

        /// <summary>
        /// only test data to be added to the global datamodel
        /// </summary>
        private readonly ObservableCollection<Desktop> _desktops;
        private readonly ObservableCollection<Credentials> _users;
        private IList<object> _selectedDesktops;
        private SessionModel SessionModel { get; set; }

        private RelayCommand ConnectTestCommand { get; set; }
        private RelayCommand DisconnectTestCommand { get; set; }
        private RelayCommand DesktopsTestCommand { get; set; }

        public RelayCommand AddDesktopCommand { get; set; }
        public RelayCommand EditDesktopCommand { get; set; }
        public RelayCommand DeleteDesktopCommand { get; set; }


        public ICommand StressTestCommand { get; private set; }
        public ICommand GoHomeCommand { get; private set; }

        public IRdpConnectionFactory RdpConnectionFactory { private get; set; }
        public ObservableCollection<Desktop> Desktops 
        { 
            get 
            {
                if (null != this.DataModel)
                {
                    return this.DataModel.Desktops;
                }
                else
                {
                    return this._desktops;
                }
            } 
        }
        public ObservableCollection<Credentials> Users 
        { 
            get 
            {
                if (null != this.DataModel)
                {
                    return this.DataModel.Credentials;
                }
                else
                {
                    return this._users;
                }
            } 
        }

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
            DisconnectTestCommand = new RelayCommand(new Action<object>(DisconnectTests));
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
            _testDisconnectItem = new SegoeGlyphBarButtonModel(SegoeGlyph.People, DisconnectTestCommand, "DisconnectTests",
                BarItemModel.ItemAlignment.Right);
            _testDesktopsItem = new SegoeGlyphBarButtonModel(SegoeGlyph.People, DesktopsTestCommand, "DesktopsTests",
                BarItemModel.ItemAlignment.Right);

            _desktops = new ObservableCollection<Desktop>();
            _users = new ObservableCollection<Credentials>();
            _selectedDesktops = null;
            this.SessionModel = null;
        }

        private void AddDesktopCommandExecute(object o)
        {
            AddDesktopViewModelArgs args = new AddDesktopViewModelArgs();
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
            Contract.Requires(null != this.SelectedDesktops);

            if (this.SelectedDesktops.Count > 0) {
                EditDesktopViewModelArgs args = new EditDesktopViewModelArgs(this.SelectedDesktops[0] as Desktop);
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

        /// <summary>
        /// execute delete
        /// </summary>
        /// <param name="o">optional parameter</param>
        private void DeleteDesktopCommandExecute(object o)
        {
            Debug.WriteLine("Delete Desktop(s)!");            
            this.NavigationService.PushModalView("DeleteDesktopsView", 
                new DeleteDesktopsArgs(this.SelectedDesktops));
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
            Contract.Requires(null != activationParameter as TestsViewModelArgs);

            TestsViewModelArgs args = activationParameter as TestsViewModelArgs;
            _connectionInformation = new ConnectionInformation()
            {
                Desktop = args.Desktop,
                Credentials = args.Credentials
            };

            LoadTestData();
        }

        protected override void OnDismissed()
        {
            ResetTestData();
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
        /// This test verifies the sequence Add desktop, connect
        ///     The tests sets the SessionModel and should be followed by disconnect tests
        /// </summary>
        /// <param name="o">test parameter</param>
        private void ConnectTests(object o)
        {
            //Contract.Requires(null != _connectionInformation);

            //Debug.WriteLine("Running connect tests  ........");
            //Credentials testUser = new Credentials() { Username = "test_user", Domain = "", Password = "test_password", HaveBeenPersisted = false };
            //AddOrEditDesktopViewModel vm = new AddOrEditDesktopViewModel();
            //Desktop testDesktop;
            //AddOrEditDesktopViewModelArgs args;
            //string returnHostName = "some_host";
            //string expectedHostName;
            
            //this.Desktops.Clear();

            //// add desktop
            //Debug.WriteLine("   ....... Adding a desktop");
            //args = new AddOrEditDesktopViewModelArgs(null, testUser, true);

            //((IViewModel)vm).Presenting(null, args, null);
            //vm.Host = "saved_new_host";
            //expectedHostName = vm.Host;
            //vm.SaveCommand.Execute(null);
            //RdTrace.IfCondThrow(returnHostName != expectedHostName, "Add desktop and save should save host name!");

            //// edit desktop - will use _connectionInformation with a valid desktop/user
            //Debug.WriteLine("   ....... Editing the desktop, save changes");
            //RdTrace.IfCondThrow(this.Desktops.Count <= 0, "At least one desktop should exist for the edit test!");
            //testDesktop = this.Desktops[0];

            //args = new AddOrEditDesktopViewModelArgs(testDesktop, _connectionInformation.Credentials, false);

            //
            // TODO:    REFACTOR THIS!
            //          The IViewModel contract requires a valid navigation service object.
            //          This particular view model does not participate in the navigation service
            //          activities and must not implement the IViewModel interface.
            //
            //((IViewModel)vm).Presenting(null, args, null);
            //vm.Host = _connectionInformation.Desktop.HostName;
            //expectedHostName = vm.Host;
            //vm.SaveCommand.Execute(null);
            //RdTrace.IfCondThrow(returnHostName != expectedHostName, "Edit desktop and save should save host name!");
            //RdTrace.IfCondThrow(testDesktop.HostName != expectedHostName, "Edit desktop and save should save update desktop!");

            //// connect using saved desktop
            //this.SessionModel = new SessionModel(RdpConnectionFactory);
            //ConnectionInformation newConnectionInformation = new ConnectionInformation() { Desktop = testDesktop, Credentials = _connectionInformation.Credentials };
            //this.SessionModel.Connect(_connectionInformation);

            //// show off - until invoking Disconnect tests

            //Debug.WriteLine("Running connect tests  ........ COMPLETED , without errors");
        }

        /// <summary>
        /// This test verifies the sequence disconnect - delete desktop 
        /// should follow after Connect test to make a visible impact, and will reset the SessionModel
        /// </summary>
        /// <param name="o">test parameter</param>
        private void DisconnectTests(object o)
        {
            Debug.WriteLine("Running disconnect tests  ........");
            if (this.SessionModel != null) 
            { 
                // disconnect
                this.SessionModel.Disconnect();
                this.SessionModel = null; 

                // delete single desktop - should have no desktops left
                Debug.WriteLine("   ....... Delete desktop");
                List<object> deleteList = new List<object>();
                deleteList.Add(this.Desktops[0]);

                // TODO: should call delete on ConnectionCenter vm 
                this.SelectedDesktops = deleteList;
                this.DeleteDesktopCommand.Execute("false");

                Debug.WriteLine("Running disconnect tests  ........ COMPLETED , without errors");
            }
            else
            {
                Debug.WriteLine("Running  disconnect tests  ........ NOP - should connect first");
            }


        }

        /// <summary>
        /// This test verifies Add/Edit/Delete desktops functionality. The changes should be visible in the list of existing desktops after completion.
        /// </summary>
        /// <param name="o"> test parameter</param>
        private void DesktopTests(object o)
        {
            //Debug.WriteLine("Running desktop management tests   .......");
            //bool delegateCalled = false;
            //string returnHostName="some_host";
            //string expectedHostName;
            //AddOrEditDesktopViewModel vm = new AddOrEditDesktopViewModel();

            //Desktop testDesktop;
            //Credentials testUser = new Credentials() { Username = "test_user", Domain = "", Password = "test_password", HaveBeenPersisted = false };

            //// Test 0: delete selected desktops, recommended to have a few desktops in the list
            //// TODO : this test should use the ConnectionCenterViewModel - simulating it for now in the TestViewModel.
            //Debug.WriteLine("   ....... Test 0 - Delete list of desktops");
            //List<object> deleteList = new List<object>();
            //for (int i = 0; i < this.Desktops.Count; i += 2)
            //{
            //    // mark one in two for deletion
            //    deleteList.Add(this.Desktops[i]);
            //}
            //// TODO: should call delete on ConnectionCenter vm 
            //this.SelectedDesktops = deleteList;
            //this.DeleteDesktopCommand.Execute("false");

            //// Test 1: add desktop, cancel
            //Debug.WriteLine("   ....... Test 1 - Add desktop, cancel changes");
            //AddOrEditDesktopViewModelArgs args = new AddOrEditDesktopViewModelArgs(null, testUser, true);

            //
            // TODO:    REFACTOR THIS!
            //          The IViewModel contract requires a valid navigation service object.
            //          This particular view model does not participate in the navigation service
            //          activities and must not implement the IViewModel interface.
            //
            //((IViewModel)vm).Presenting(null, args, null);
            //vm.Host = "unsaved_new_host";
            //vm.CancelCommand.Execute(null);           
            //RdTrace.IfCondThrow(delegateCalled, "Add desktop and cancel should not call saveDelegate!");


            //// Test 2: add desktop, save
            //Debug.WriteLine("   ....... Test 2 - Add desktop, save changes");
            //delegateCalled = false;
            //args = new AddOrEditDesktopViewModelArgs(null, testUser, true);

            //
            // TODO:    REFACTOR THIS!
            //          The IViewModel contract requires a valid navigation service object.
            //          This particular view model does not participate in the navigation service
            //          activities and must not implement the IViewModel interface.
            //
            //((IViewModel)vm).Presenting(null, args, null);
            //vm.Host = "saved_new_host";
            //expectedHostName = vm.Host;
            //vm.SaveCommand.Execute(null);
            //RdTrace.IfCondThrow(!delegateCalled, "Add desktop and save should call saveDelegate!");
            //RdTrace.IfCondThrow(returnHostName != expectedHostName, "Add desktop and save should save host name!");


            //// Test 3: edit desktop, cancel
            //Debug.WriteLine("   ....... Test 3 - Edit desktop, cancel changes");
            //delegateCalled = false;
            //RdTrace.IfCondThrow(this.Desktops.Count <= 0 , "At least one desktop should exist for the edit test!");
            //testDesktop = this.Desktops[0];
            //args = new AddOrEditDesktopViewModelArgs(testDesktop, testUser, false);

            //((IViewModel)vm).Presenting(null, args, null);
            //vm.Host = "not_updated_host";
            //expectedHostName = testDesktop.HostName;
            //vm.CancelCommand.Execute(null);
            //RdTrace.IfCondThrow(delegateCalled, "Edit desktop and cancel should not call saveDelegate!");
            //RdTrace.IfCondThrow(testDesktop.HostName != expectedHostName, "Edit desktop and cancel should not change host name!");

            //// Test 4: edit desktop, save
            //Debug.WriteLine("   ....... Test 4 - Edit desktop, save changes");
            //delegateCalled = false;
            //RdTrace.IfCondThrow(this.Desktops.Count <= 0, "At least one desktop should exist for the edit test!");
            //testDesktop = this.Desktops[0];

            //args = new AddOrEditDesktopViewModelArgs(testDesktop, testUser, false);

            //((IViewModel)vm).Presenting(null, args, null);
            //vm.Host = "updated_host";
            //expectedHostName = vm.Host;
            //vm.SaveCommand.Execute(null);
            //RdTrace.IfCondThrow(!delegateCalled, "Edit desktop and save should call saveDelegate!");
            //RdTrace.IfCondThrow(returnHostName != expectedHostName, "Edit desktop and save should save host name!");
            //RdTrace.IfCondThrow(testDesktop.HostName != expectedHostName, "Edit desktop and save should save update desktop!");

            //Debug.WriteLine("Running desktop management tests   ....... COMPLETED , without errors");
        }

        private void GoHome(object o)
        {
            NavigationService.NavigateToView("ConnectionCenterView", null);
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
                _testDisconnectItem,
                _testDesktopsItem
            };
        }

        /// <summary>
        /// append test data to the actual DataModel
        /// </summary>
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

            if (null != DataModel)
            {
                foreach (Desktop desktop in _desktops)
                {
                    this.DataModel.Desktops.Add(desktop);
                }

                foreach (Credentials creds in _users)
                {
                    this.DataModel.Credentials.Add(creds);
                }
            }

            AddDesktopCommand.EmitCanExecuteChanged();
        }

        /// <summary>
        /// removing testData, but preserving everything else
        /// should be used when dismissing this view.
        /// </summary>
        private void ResetTestData()
        {
            Contract.Requires(null != _desktops);
            Contract.Requires(null != _users);

            if (null != DataModel) 
            { 
                foreach (Desktop desktop in _desktops)
                {
                    this.DataModel.Desktops.Remove(desktop);
                }

                foreach (Credentials creds in _users)
                {
                    this.DataModel.Credentials.Remove(creds);
                }
            }
        }


    }
}
