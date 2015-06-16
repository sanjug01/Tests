using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.ViewModels;
using Windows.Foundation;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class ConnectionBarViewModelTests
    {

        ConnectionBarViewModel _vvm;
        double _connectionBarWidth;
        double _containerWdith;
        
        [TestInitialize]
        public void SetUpTest()
        {
            _vvm = new ConnectionBarViewModel();
            _connectionBarWidth = 40;
            _containerWdith = 1000;
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _vvm = null;
        }

        [TestMethod]
        public void ConnectionBarViewModel_Default_Position_Is_Center()
        {
            Assert.AreEqual(0, _vvm.Position);
        }


        [TestMethod]
        public void ConnectionBarViewModel_Move_Updates_Position()
        {
            _vvm.MoveConnectionBar(2.0, _connectionBarWidth, _containerWdith);
            Assert.AreEqual(2.0, _vvm.Position);

            _vvm.MoveConnectionBar(-2.0, _connectionBarWidth, _containerWdith);
            Assert.AreEqual(0.0, _vvm.Position);

            _vvm.MoveConnectionBar(10.0, _connectionBarWidth, _containerWdith);
            Assert.AreEqual(10.0, _vvm.Position);

            _vvm.MoveConnectionBar(-12.0, _connectionBarWidth, _containerWdith);
            Assert.AreEqual(-2.0, _vvm.Position);
        }


      [TestMethod]
       public void ConnectionBarViewModel_Move_Stays_Within_Viewport()
        {

            double maxLeft = -((_containerWdith / 2) - (_connectionBarWidth / 2));
            double maxRight = ((_containerWdith / 2) - (_connectionBarWidth / 2));

            _vvm.MoveConnectionBar(700, _connectionBarWidth, _containerWdith);
            Assert.AreEqual(maxRight, _vvm.Position);

            _vvm.MoveConnectionBar(-1700, _connectionBarWidth, _containerWdith);
            Assert.AreEqual(maxLeft, _vvm.Position);

        }
    }
}
