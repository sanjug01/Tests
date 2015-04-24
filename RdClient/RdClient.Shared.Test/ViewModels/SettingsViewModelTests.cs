using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.Test.Data;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace RdClient.Shared.Test.ViewModels
{
    /*
[TestClass]
public class SettingsViewModelTests
{

private TestData _testData;
private ApplicationDataModel _dataModel;
private Mock.NavigationService _navService;
private Mock.ModalPresentationContext _context;
private SettingsViewModel _vm;

[TestInitialize]
public void TestSetup()
{
    _testData = new TestData();
    _navService = new Mock.NavigationService();
    _dataModel = new ApplicationDataModel()
    {
        RootFolder = new MemoryStorageFolder(),
        ModelSerializer = new SerializableModelSerializer()
    };
    _vm = new SettingsViewModel();
    ((IDataModelSite)_vm).SetDataModel(_dataModel);
    _context = new Mock.ModalPresentationContext();
    ((IViewModel)_vm).Presenting(_navService, null, null); 
}

[TestCleanup]
public void TestCleanup()
{
    _navService.Dispose();
    _context.Dispose();
    _dataModel = null;
}

[TestMethod]
public void TestGoBackCommandDismisses()
{
    ((IViewModel)_vm).Presenting(_navService, null, _context);
    _context.Expect("Dismiss", p => { return null; });
    _vm.Cancel.Execute(null);
}

[TestMethod]
public void TestHandlesBackNavigationByDismissing()
{
    ((IViewModel)_vm).Presenting(_navService, null, _context);
    _context.Expect("Dismiss", p => { return null; });
    IBackCommandArgs backArgs = new BackCommandArgs();
    Assert.IsFalse(backArgs.Handled);
    (_vm as IViewModel).NavigatingBack(backArgs);
    Assert.IsTrue(backArgs.Handled);
}

[TestMethod]
public void TestSettingsLoadedFromDataModel()
{
    Assert.AreEqual(_vm.GeneralSettings, _dataModel.Settings);
}

[TestMethod]
public void TestAddUserCommandShowsAddUserView()
{
    _navService.Expect("PushModalView", new List<object>() { "AddUserView", null, null }, 0);
    _vm.AddUserCommand.Execute(null);
}

[TestMethod]
public void AddCredentialToDataModelAddsMatchingCredentialViewModel()
{
    CredentialsModel cred = _testData.NewValidCredential().Model;
    _dataModel.Credentials.AddNewModel(cred);
    Assert.AreEqual(cred, _vm.CredentialsViewModels[0].Credentials);
}

[TestMethod]
public void GeneralSettingsSetToMatchDataModel()
{
    Assert.AreEqual(_dataModel.Settings, _vm.GeneralSettings);
}

[TestMethod]
public void OnPresentingSetsGeneralSettings()
{
    ApplicationDataModel newDataModel = new ApplicationDataModel()
    {
        RootFolder = new MemoryStorageFolder(),
        ModelSerializer = new SerializableModelSerializer()
    };
    ((IDataModelSite)_vm).SetDataModel(newDataModel);
    ((IViewModel)_vm).Presenting(_navService, null, null); 
    Assert.AreEqual(newDataModel.Settings, _vm.GeneralSettings);
}

[TestMethod]
public void NewDataModelRecreatesAllCredentialViewModels()
{
    ApplicationDataModel newDataModel = new ApplicationDataModel()
    {
        RootFolder = new MemoryStorageFolder(),
        ModelSerializer = new SerializableModelSerializer()
    };
    IList<IModelContainer<CredentialsModel>> creds = _testData.NewSmallListOfCredentials();

    foreach (IModelContainer<CredentialsModel> cred in creds)
    {
        newDataModel.Credentials.AddNewModel(cred.Model);
    }
    //
    // Dismiss, change the data model, and present again - the data model is changed by a navigation extention,
    // that sets it before the view model is presented.
    //
    ((IViewModel)_vm).Dismissing();
    ((IDataModelSite)_vm).SetDataModel(newDataModel);
    ((IViewModel)_vm).Presenting(_navService, null, null);

    Assert.AreEqual(creds.Count, _vm.CredentialsViewModels.Count);
    foreach (IModelContainer<CredentialsModel> cred in creds)
    {
        _vm.CredentialsViewModels.Any(vm => cred.Model.Equals(vm.Credentials));
    }
}
}
*/
}
