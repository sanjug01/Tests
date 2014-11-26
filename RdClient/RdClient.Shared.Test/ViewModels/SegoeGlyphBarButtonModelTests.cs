namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.ViewModels;
    using System;

    [TestClass]
    public sealed class SegoeGlyphBarButtonModelTests
    {
        [TestMethod]
        public void CreateModelsForAllGlyphsValues_CorrectModelsCreated()
        {
            foreach (SegoeGlyph glyph in Enum.GetValues(typeof(SegoeGlyph)))
            {
                SegoeGlyphBarButtonModel model = new SegoeGlyphBarButtonModel(glyph, new RelayCommand(o => { }), glyph.ToString());
                Assert.AreEqual(glyph, model.Glyph);
            }
        }
    }
}
