using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public interface ISymbolBarItemModel
    {
        bool CanExecute { get; set; }
        ICommand Command { get; set; }
        object CommandParameter { get; set; }
        SegoeGlyph Glyph { get; set; }
    }
}