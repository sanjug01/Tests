using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Models.Viewport;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Windows.Foundation;

namespace RdClient.Shared.ViewModels
{
    public class SymbolBarSliderButtonModel : MutableObject, ISymbolBarItemModel
    {

        private IViewport _viewport;
        public IViewport Viewport { set { _viewport = value; } }

        public bool CanExecute
        {
            get { return _command.CanExecute(null); }
            set { }
        }

        private readonly RelayCommand _command = new RelayCommand(o => { }, 
            o => {
                return true;
            });
        public ICommand Command
        {
            get { return _command; }
            set { }
        }

        public object CommandParameter
        {
            get { return null; }
            set { }
        }

        private SegoeGlyph _glyph = SegoeGlyph.Home;
        public SegoeGlyph Glyph
        {
            get { return _glyph; }
            set { _glyph = value; }
        }

        private Double _value = 0.0;
        public Double Value
        {
            get { return _value; }
            set
            {
                if(_viewport != null)
                {
                    double factor = 3.0 * value / 100.0;


                    Point center = new Point(_viewport.Size.Width / 2.0, _viewport.Size.Height / 2.0);
                    _viewport.Set(1.0 + factor, center);
                }

                SetProperty(ref _value, value);
            }
        }
    }
}
