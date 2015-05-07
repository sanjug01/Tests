namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Models;

    [TestClass]
    public sealed class GeneralSettingsModelTests
    {
        [TestMethod]
        public void GeneralSettings_DefaultValues()
        {
            GeneralSettings settings = new GeneralSettings();
            Assert.IsTrue(settings.SendFeeback);
            Assert.IsTrue(settings.UseThumbnails);
        }
    }
}
