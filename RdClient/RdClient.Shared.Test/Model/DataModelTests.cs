using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using RdClient.Shared.Models;


namespace RdClient.Shared.Test.Model
{
    [TestClass]
    public class DataModelTests : IDataModelTests
    {
        protected override async Task<IDataModel> CreateDataModel(Models.IDataStorage storage)
        {
            IDataModel dataModel = new DataModel();
            dataModel.Storage = storage;
            await dataModel.LoadFromStorage();
            return dataModel;
        }
    }
}
