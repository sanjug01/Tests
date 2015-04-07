namespace RdClient.Shared.ViewModels
{
    using System.Windows.Input;

    /// <summary>
    /// Subset of Segoe UI app bar icon glyphs.
    /// http://msdn.microsoft.com/en-us/library/windows/apps/jj841126.aspx
    /// </summary>
    public enum SegoeGlyph : ushort
    {
        Trash = 0xE107,
        Home = 0xE10F,
        Forward = 0xe111,
        Back = 0xE112,
        Settings = 0xE115,
        Find = 0xE11A,
        People = 0xE125,
        Add = 0xE109,
        Edit = 0xE104,
        MultiSelection= 0xE762
    }

    /// <summary>
    /// Model of an application bar button showing a Segoe UI symbol.
    /// </summary>
    public sealed class SegoeGlyphBarButtonModel : BarButtonModel
    {
        private readonly SegoeGlyph _glyph;

        public SegoeGlyph Glyph { get { return _glyph; } }

        public SegoeGlyphBarButtonModel(SegoeGlyph glyph, ICommand command, string label, ItemAlignment alignment = ItemAlignment.Left)
            : base(command, label, alignment)
        {
            _glyph = glyph;
        }
    }
}
