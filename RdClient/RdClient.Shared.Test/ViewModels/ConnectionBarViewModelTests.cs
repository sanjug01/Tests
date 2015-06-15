using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.ViewModels;
using Windows.Foundation;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class ConnectionBarViewModelTests
    {

        ConnectionBarViewModel _vvm;
        Mock.Viewport _viewport;
        double _connectionBarWidth;
        
        [TestInitialize]
        public void SetUpTest()
        {
            _vvm = new ConnectionBarViewModel();
            _viewport = new Mock.Viewport();
            _viewport.Size = new Size(800, 800);
            _vvm.Viewport = _viewport;
            _connectionBarWidth = 40;
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _vvm = null;
            _viewport = null;
        }

        [TestMethod]
        public void ConnectionBarViewModel_Default_Position_Is_Center()
        {
            Assert.AreEqual(0, _vvm.ConnectionBarPosition);
        }


        [TestMethod]
        public void ConnectionBarViewModel_Move_Updates_Position()
        {
            _vvm.MoveConnectionBar(2.0, _connectionBarWidth);
            Assert.AreEqual(2.0, _vvm.ConnectionBarPosition);

            _vvm.MoveConnectionBar(-2.0, _connectionBarWidth);
            Assert.AreEqual(0.0, _vvm.ConnectionBarPosition);

            _vvm.MoveConnectionBar(10.0, _connectionBarWidth);
            Assert.AreEqual(10.0, _vvm.ConnectionBarPosition);

            _vvm.MoveConnectionBar(-12.0, _connectionBarWidth);
            Assert.AreEqual(-2.0, _vvm.ConnectionBarPosition);
        }


      [TestMethod]
       public void ConnectionBarViewModel_Move_Stays_Within_Viewport()
        {
            double viewPortWidth = _viewport.Size.Width;

            double maxLeft = -((_viewport.Size.Width / 2) - (_connectionBarWidth / 2));
            double maxRight = ((_viewport.Size.Width / 2) - (_connectionBarWidth / 2));

            _vvm.MoveConnectionBar(700, _connectionBarWidth);
            Assert.AreEqual(maxRight, _vvm.ConnectionBarPosition);

            _vvm.MoveConnectionBar(-1700, _connectionBarWidth);
            Assert.AreEqual(maxLeft, _vvm.ConnectionBarPosition);

        }
    }
}
