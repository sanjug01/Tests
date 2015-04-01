namespace RdClient.Shared.Test.Input.Keyboard
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Input.Keyboard;

    [TestClass]
    public sealed class KeystrokeEventArgsTests
    {
        [TestMethod]
        public void NewKeystrokeEventArgs_CorrectPropertiesSet()
        {
            KeystrokeEventArgs e = new KeystrokeEventArgs(32, true, true, true);
            Assert.AreEqual(32, e.KeyCode);
            Assert.IsTrue(e.IsScanCode);
            Assert.IsTrue(e.IsExtendedKey);
            Assert.IsTrue(e.IsKeyReleased);
        }
    }
}
