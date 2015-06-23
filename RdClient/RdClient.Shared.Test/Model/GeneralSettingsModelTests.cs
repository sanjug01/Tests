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
#if DEBUG
            Assert.IsFalse(settings.SendFeedback);
#else
            Assert.IsTrue(settings.SendFeedback);
#endif
            Assert.IsTrue(settings.UseThumbnails);
        }
    }
}
