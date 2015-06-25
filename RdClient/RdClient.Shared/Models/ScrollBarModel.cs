using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using RdClient.Shared.Models.Viewport;
using System;
using System.Diagnostics;
using Windows.Devices.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace RdClient.Shared.Models
{
    public class ScrollBarModel : MutableObject, IScrollBarModel
    {
        private Visibility _visibility = Visibility.Visible;

        public void SetScrollbarVisibility(Visibility visibility)
        {
            _visibility = visibility;
            EmitPropertyChanged("VisibilityHorizontal");
            EmitPropertyChanged("VisibilityVertical");
            EmitPropertyChanged("VisibilityCorner");
        }

        public double MinimumHorizontal
        {
            get
            {
                return 0;
            }
        }

        public double MinimumVertical
        {
            get
            {
                return 0;
            }
        }

        public double MaximumHorizontal
        {
            get
            {
                return CheckViewport(() => _viewport.SessionPanel.Width - _viewport.Size.Width, 0);
            }
        }

        public double MaximumVertical
        {
            get
            {
                return CheckViewport(() => _viewport.SessionPanel.Height- _viewport.Size.Height, 0);
            }
        }


        public double ViewportWidth
        {
            get
            {
                return CheckViewport(() => _viewport.Size.Width, 0);
            }
        }

        public double ViewportHeight
        {
            get
            {
                return CheckViewport(() => _viewport.Size.Height, 0);
            }
        }

        private IViewport _viewport;
        public IViewport Viewport
        {
            set
            {
                _viewport = value;
                if(_viewport != null)
                {
                    _viewport.Changed += OnViewportChanged;
                    this.OnViewportChanged(this, null);
                }
            }
        }

        private double _valueHorziontal;
        public double ValueHorziontal
        {
            get
            {
                return _valueHorziontal;
            }
            set
            {
                _viewport.SetPan(value, _viewport.Offset.Y);
                SetProperty(ref _valueHorziontal, value);
            }
        }

        private double _valueVertical;
        public double ValueVertical
        {
            get
            {
                return _valueVertical;
            }
            set
            {
                _viewport.SetPan(_viewport.Offset.X, value);
                SetProperty(ref _valueVertical, value);
            }
        }

        public Visibility VisibilityHorizontal
        {
            get
            {
                return CheckViewport(() =>
                {
                    if (_visibility == Visibility.Visible
                        && _viewport.Size.Width + _horizontalScrollBarWidth < _viewport.SessionPanel.Width)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }, Visibility.Collapsed);
            }
        }

        public Visibility VisibilityVertical
        {
            get
            {
                return CheckViewport(() =>
                {
                    if (_visibility == Visibility.Visible
                        && _viewport.Size.Height + _verticalScrollBarWidth < _viewport.SessionPanel.Height)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }, Visibility.Collapsed);

            }
        }

        public Visibility VisibilityCorner
        {
            get
            {
                if (_visibility == Visibility.Visible
                    && this.IndicatorMode == ScrollingIndicatorMode.MouseIndicator
                    && (this.VisibilityHorizontal == Visibility.Visible || this.VisibilityVertical == Visibility.Visible))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        private double _horizontalScrollBarWidth = 0;
        public double HorizontalScrollBarWidth
        {
            set
            {
                _horizontalScrollBarWidth = value;
            }
        }

        private double _verticalScrollBarWidth = 0;
        public double VerticalScrollBarWidth
        {
            set
            {
                _verticalScrollBarWidth = value;
            }
        }

        private void OnViewportChanged(object sender, EventArgs e)
        {
            CheckViewport<object>(() => 
            {
                this.ValueHorziontal = _viewport.Offset.X;
                this.ValueVertical = _viewport.Offset.Y;

                EmitPropertyChanged("MinimumVertical");
                EmitPropertyChanged("MaximumVertical");
                EmitPropertyChanged("MinimumHorizontal");
                EmitPropertyChanged("MaximumHorizontal");
                EmitPropertyChanged("ViewportWidth");
                EmitPropertyChanged("ViewportHeight");
                EmitPropertyChanged("VisibilityHorizontal");
                EmitPropertyChanged("VisibilityVertical");
                EmitPropertyChanged("VisibilityCorner");
                return null;
            }, null);
        }

        public ScrollBarModel()
        {

        }

        private T CheckViewport<T>(Func<T> action, T def)
        {
            T result = def;

            if(_viewport != null)
            {
                result = action();
            }

            return result;
        }

        private ScrollingIndicatorMode _indicatorMode = ScrollingIndicatorMode.None;
        public ScrollingIndicatorMode IndicatorMode
        {
            get
            {
                return _indicatorMode;
            }
            private set
            {
                EmitPropertyChanged("VisibilityCorner");

                SetProperty(ref _indicatorMode, value);
            }
        }

        public void OnPointerChanged(object sender, IPointerEventBase e)
        {
            if (e is IPointerRoutedEventProperties)
            {
                IPointerRoutedEventProperties iprep = (IPointerRoutedEventProperties)e;
                if (iprep.DeviceType == PointerDeviceType.Mouse)
                {
                    this.IndicatorMode = ScrollingIndicatorMode.MouseIndicator;
                }
                else if (iprep.DeviceType == PointerDeviceType.Touch)
                {
                    this.IndicatorMode = ScrollingIndicatorMode.TouchIndicator;
                }
            }
        }
    }
}
