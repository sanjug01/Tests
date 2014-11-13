using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using RdClient.Shared.Models;


namespace RdClient.Shared.Test.Model
{
    [TestClass]
    public class DataModelTests : IDataModelTests
    {
        protected override IDataModel GetDataModel(Models.IDataStorage storage)
        {
            return DataModel.NewDataModel(storage).Result;
        }
    }
}
