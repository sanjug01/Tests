namespace RdClient.Shared.Test.Input.Keyboard
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Keyboard;

    [TestClass]
    public sealed class PhysicalKeyDataContainerTests
    {
        private sealed class KeyData : IPhysicalKeyData
        {
        }

        [TestMethod]
        public void NewKeyDataContainer_NoKeyData()
        {
            PhysicalKeyDataContainer container = new PhysicalKeyDataContainer();
            Assert.IsNull(container.KeyData);
        }

        [TestMethod]
        public void NewKeyDataContainer_SetKeyData_CorrectValueSet()
        {
            PhysicalKeyDataContainer container = new PhysicalKeyDataContainer();
            IPhysicalKeyData keyData = new KeyData();

            container.KeyData = keyData;
            Assert.AreSame(keyData, container.KeyData);
        }

        [TestMethod]
        public void KeyDataContainerWithData_DoIfDifferentType_NotCalled()
        {
            bool called = false;

            PhysicalKeyDataContainer container = new PhysicalKeyDataContainer() { KeyData = new KeyData() };
            container.DoIf<IWindowSize>(pss =>
            {
                called = true;
            });
            Assert.IsFalse(called);
        }

        [TestMethod]
        public void KeyDataContainerWithData_DoIfSameType_Called()
        {
            KeyData callParam = null;

            PhysicalKeyDataContainer container = new PhysicalKeyDataContainer() { KeyData = new KeyData() };
            container.DoIf<KeyData>(kd =>
            {
                callParam = kd;
            });
            Assert.IsNotNull(callParam);
            Assert.AreSame(callParam, container.KeyData);
        }
    }
}
