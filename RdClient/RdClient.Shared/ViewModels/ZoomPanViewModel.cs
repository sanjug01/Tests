namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Input.Mouse;
    using RdClient.Shared.Input.ZoomPan;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation.Extensions;    
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Windows.Input;
    using Windows.Foundation;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Imaging;

    public class ZoomTransform : IZoomPanTransform
    {
        public TransformType TransformType { get; private set; }

        public ZoomTransform(TransformType type)
        {
            TransformType = type;
        }
    }

    public class CustomZoomTransform : ICustomZoomTransform
    {
        public CustomZoomTransform(double centerX, double centerY, double scaleX, double scaleY)
        {
            TransformType = TransformType .ZoomCustom;
            CenterX = centerX;
            CenterY = centerY;
            ScaleX = scaleX;
            ScaleY = scaleY;
        }

        public TransformType TransformType { get; private set; }
        public double CenterX { get; private set; }
        public double CenterY { get; private set; }
        public double ScaleX { get; private set; }
        public double ScaleY { get; private set; }
    }

    public class PanTransform : IPanTransform
    {
        public TransformType TransformType { get; private set; }
        public const double DefaultPan = 50.0;
        public double X { get; private set; }
        public double Y { get; private set; }
        public PanTransform(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.TransformType = TransformType.Pan;
        }
    }

    // specific zoom/pan instances to be used as parameters
    public sealed class ZoomInTransform : ZoomTransform
    {
        public ZoomInTransform() : base (TransformType.ZoomIn) { }
    }

    public sealed class ZoomOutTransform : ZoomTransform
    {
        public ZoomOutTransform() : base (TransformType.ZoomOut) { }
    }

    public sealed class PanLeftTransform : PanTransform
    {
        public PanLeftTransform() : base (-PanTransform.DefaultPan, 0.0 ) { }
    }

    public sealed class PanRightTransform : PanTransform
    {
        public PanRightTransform() : base (PanTransform.DefaultPan, 0.0 ) { }
    }

    public sealed class PanUpTransform : PanTransform
    {
        public PanUpTransform() : base (0.0, PanTransform.DefaultPan ) { }
    }

    public sealed class PanDownTransform : PanTransform
    {
        public PanDownTransform() : base (0.0, - PanTransform.DefaultPan ) { }
    }


    public sealed class ZoomPanViewModel : MutableObject, IZoomPanManipulator, IZoomPanViewModel
    {
        private IZoomPanTransform _zoomPanTransform;

        private const double MAX_ZOOM_FACTOR = 2.5;
        private const double MIN_ZOOM_FACTOR = 1.0;
        private const double ZOOM_INCREMENT = 0.125;

        private double _scaleCenterX;
        private double _scaleCenterY;
        private double _scaleXFrom;
        private double _scaleXTo;
        private double _scaleYFrom;
        private double _scaleYTo;
        private double _translateXFrom;
        private double _translateXTo;
        private double _translateYFrom;
        private double _translateYTo;
        private ZoomPanState _zoomPanState;

        //private Point _viewPosition;
        //private Size _viewSize;
        private Rect _viewRect;
        private Rect _windowRect;

        public double ScaleCenterX
        {
            get { return _scaleCenterX; }
            set { this.SetProperty(ref _scaleCenterX, value); }
        }
        public double ScaleCenterY
        {
            get { return _scaleCenterY; }
            set { this.SetProperty(ref _scaleCenterY, value); }
        }
        public double ScaleXFrom
        {
            get { return _scaleXFrom; }
            set { this.SetProperty(ref _scaleXFrom, value); }
        }
        public double ScaleXTo
        {
            get { return _scaleXTo; }
            set { this.SetProperty(ref _scaleXTo, value); }
        }
        private double ScaleX { get { return _scaleXTo; } }

        public double ScaleYFrom
        {
            get { return _scaleYFrom; }
            set { this.SetProperty(ref _scaleYFrom, value); }
        }
        public double ScaleYTo
        {
            get { return _scaleYTo; }
            set { this.SetProperty(ref _scaleYTo, value); }
        }
        private double ScaleY { get { return _scaleYTo; } }

        public double TranslateXFrom
        {
            get { return _translateXFrom; }
            set { this.SetProperty(ref _translateXFrom, value); }
        }
        public double TranslateXTo 
        {
            get { return _translateXTo; }
            set { this.SetProperty(ref _translateXTo, value); }
        }
        public double TranslateYFrom
        {
            get { return _translateYFrom; }
            set { this.SetProperty(ref _translateYFrom, value); }
        }
        public double TranslateYTo
        {
            get { return _translateYTo; }
            set { this.SetProperty(ref _translateYTo, value); }
        }
        public Rect WindowRect 
        {
            get { return _windowRect; } 
            set
            {            
                _windowRect = value;
                this.ScaleCenterX = (value.Right - value.Left) / 2.0;
                this.ScaleCenterY = (value.Bottom - value.Top) / 2.0; 
                UpdateViewRect(); 
            }
        }

        public Point TranslatePosition(Point visiblePoint)
        {
            Contract.Requires(0 != this.ScaleX, "Invalid scale factor!");
            Contract.Requires(0 != this.ScaleY, "Invalid scale factor!");

            return new Point(
                _viewRect.X + visiblePoint.X / this.ScaleX,
                _viewRect.Y + visiblePoint.Y / this.ScaleY
                );
        }

        public IZoomPanTransform ZoomPanTransform
        {
            get { return _zoomPanTransform; }
            private set { this.SetProperty<IZoomPanTransform>(ref _zoomPanTransform, value); }
        }

        public void HandlePanChange(object sender, PanEventArgs e)
        {
            if (null != e && IsZoomed())
            {
                this.ApplyPanTransform(e.DeltaX, e.DeltaY);
                this.ZoomPanTransform = new PanTransform(e.DeltaX, e.DeltaX);
            }
        }

        public void HandleScaleChange(object sender, ZoomEventArgs e)
        {
            if (null != e)
            {
                if(e.FromLength > 0 && e.ToLength > 0)
                {
                    double targetXScale = this.ScaleX;
                    double targetYScale = this.ScaleY;
                    bool updateRequired = false;

                    if(e.ToLength > e.FromLength + GlobalConstants.TouchZoomDeltaThreshold)
                    {
                        // enlarging
                        updateRequired = (targetXScale < MAX_ZOOM_FACTOR || targetYScale < MAX_ZOOM_FACTOR);
                        targetXScale += ZOOM_INCREMENT;
                        targetYScale += ZOOM_INCREMENT;                        
                    }
                    else if (e.ToLength < e.FromLength - GlobalConstants.TouchZoomDeltaThreshold)
                    {
                        // shrinking
                        updateRequired = (targetXScale > MIN_ZOOM_FACTOR || targetYScale > MIN_ZOOM_FACTOR);
                        targetXScale -= ZOOM_INCREMENT;
                        targetYScale -= ZOOM_INCREMENT;
                    }

                    if (updateRequired)
                    {
                        this.ApplyZoomTransform(e.CenterX, e.CenterY, targetXScale, targetYScale);
                        this.ZoomPanTransform = new CustomZoomTransform(e.CenterX, e.CenterY, targetXScale, targetYScale);
                    }
                }
            }
        }

        public void HandleInputModeChange(object sender, InputModeChangedEventArgs e)
        {
            switch(e.Mode)
            {
                case ConsumptionMode.DirectTouch:
                case ConsumptionMode.MultiTouch:
                    if (ZoomPanState.TouchMode_MinScale != this.State && ZoomPanState.TouchMode_MaxScale != this.State)
                    {
                        // reset to default scale, and switch to touch mode
                        if(IsZoomed())
                        {
                            this.ApplyZoomOut();
                            this.ZoomPanTransform = new ZoomOutTransform();
                        }
                        this.State = ZoomPanState.TouchMode_MinScale;
                    }

                    break;
                case ConsumptionMode.Pointer:
                    if (ZoomPanState.TouchMode_MinScale == this.State || ZoomPanState.TouchMode_MaxScale == this.State)
                    {
                        // reset to default scale, and switch to pointer mode
                        if (IsZoomed())
                        {
                            this.ApplyZoomOut();
                            this.ZoomPanTransform = new ZoomOutTransform();
                        }
                        this.State = ZoomPanState.PointerMode_DefaultScale;
                    }
                    break;
            }
        }

        private readonly ICommand _toggleZoomCommand;
        public ICommand ToggleZoomCommand { get { return _toggleZoomCommand; } }
        private readonly ICommand _panCommand;
        public ICommand PanCommand { get { return _panCommand; } }

        public ZoomPanState State
        {
            get { return _zoomPanState; }
            private set { this.SetProperty(ref _zoomPanState, value); }
        }

        public ZoomPanViewModel()
        {
            _toggleZoomCommand = new RelayCommand(new Action<object>(ToggleMagnification));
            _panCommand = new RelayCommand(new Action<object>(PanTranslate));
            
            ScaleCenterX = 0.0;
            ScaleCenterX = 0.0;

            ScaleXFrom = 1.0;
            ScaleYFrom = 1.0;
            ScaleXTo = 1.0;
            ScaleYTo = 1.0;

            TranslateXFrom = 0.0;
            TranslateYFrom = 0.0;
            TranslateXTo = 0.0;
            TranslateYTo = 0.0;

            WindowRect = new Rect(0, 0, 0, 0);
            this.State = ZoomPanState.PointerMode_DefaultScale;
        }

        private void ToggleMagnification(object o)
        {
            IZoomPanTransform zoomTransform = (o as IZoomPanTransform);
            if (null !=  zoomTransform)
            {
                if(TransformType.ZoomIn == zoomTransform.TransformType)
                {
                    this.ApplyZoomIn();
                    this.State = ZoomPanState.TouchMode_MaxScale;
                }
                else if(TransformType.ZoomOut == zoomTransform.TransformType)
                {
                    this.ApplyZoomOut();
                    this.State = ZoomPanState.TouchMode_MinScale;
                }
                else if (TransformType.ZoomCustom == zoomTransform.TransformType)
                {
                    ICustomZoomTransform customZoomTransform = (ICustomZoomTransform) zoomTransform;
                    this.ApplyZoomTransform(customZoomTransform.CenterX, customZoomTransform.CenterY, customZoomTransform.ScaleX, customZoomTransform.ScaleY);
                }

                this.ZoomPanTransform = new ZoomTransform(zoomTransform.TransformType);
            }
        }

        private void PanTranslate(object o)
        {
            IPanTransform panTransform = (o as IPanTransform);
            if (null != panTransform 
                // && IsZoomed()
                )
            {
                this.ApplyPanTransform(panTransform.X, panTransform.Y);
                this.ZoomPanTransform = new PanTransform(this.TranslateXTo, this.TranslateYTo);
            }
        }

        private void ApplyZoomIn()
        {
            double targetScaleFactor = MAX_ZOOM_FACTOR;

            // reset the pan transformation
            this.TranslateXFrom = this.TranslateXTo;
            this.TranslateYFrom = this.TranslateYTo;

            this.ScaleXFrom = this.ScaleXTo;
            this.ScaleXTo = targetScaleFactor;

            this.ScaleYFrom = this.ScaleYTo;
            this.ScaleYTo = targetScaleFactor;

            this.ScaleCenterX = (WindowRect.Right - WindowRect.Left) / 2;
            this.ScaleCenterY = (WindowRect.Bottom - WindowRect.Top) / 2;

            UpdateViewRect();
        }

        private void ApplyZoomOut()
        {
            double targetScaleFactor = MIN_ZOOM_FACTOR;

            // reset the pan transformation
            this.TranslateXFrom = this.TranslateXTo;
            this.TranslateYFrom = this.TranslateYTo;

            this.ScaleXFrom = this.ScaleXTo;
            this.ScaleXTo = targetScaleFactor;

            this.ScaleYFrom = this.ScaleYTo;
            this.ScaleYTo = targetScaleFactor;

            this.ScaleCenterX = (WindowRect.Right - WindowRect.Left) / 2;
            this.ScaleCenterY = (WindowRect.Bottom - WindowRect.Top) / 2;

            //
            // Translate (through animation) the swap chain panel to fit the screen
            //
            this.TranslateXTo = WindowRect.X;
            this.TranslateYTo = WindowRect.Y;

            UpdateViewRect();
        }

        private void ApplyZoomTransform(double centerX, double centerY, double scaleX, double scaleY)
        {
            double targetScaleX = scaleX;
            double targetScaleY = scaleY;

            // overwrite center
            centerX = (WindowRect.Right - WindowRect.Left) / 2.0;
            centerY = (WindowRect.Bottom - WindowRect.Top) / 2.0;

            targetScaleX = Math.Min( Math.Max(targetScaleX, MIN_ZOOM_FACTOR), MAX_ZOOM_FACTOR );
            targetScaleY = Math.Min(Math.Max(targetScaleY, MIN_ZOOM_FACTOR), MAX_ZOOM_FACTOR);

            // reset the pan transformation
            double translateX = this.TranslateXTo + centerX - (WindowRect.Right - WindowRect.Left) / 2;
            double translateY = this.TranslateYTo + centerY - (WindowRect.Bottom - WindowRect.Top) / 2;
            this.TranslateXFrom = this.TranslateXTo = translateX;
            this.TranslateYFrom = this.TranslateYTo = translateY;

            this.ScaleXFrom = this.ScaleXTo;
            this.ScaleXTo = targetScaleX;
            this.ScaleYFrom = this.ScaleYTo;
            this.ScaleYTo = targetScaleY;

            if (!IsZoomed())
            {
                this.State = ZoomPanState.PointerMode_DefaultScale;
            }
            else
            {
                this.State = ZoomPanState.PointerMode_Zooming;
            }

            //if (this.ScaleXTo < this.ScaleXFrom || this.ScaleYTo < this.ScaleYFrom)
            {
                // shrinking may required pan adjustment
                double panXTo = this.TranslateXTo;
                double panYTo = this.TranslateYTo;
                this.ApplyPanAdjusments(ref panXTo, ref panYTo, targetScaleX, targetScaleY, ref centerX, ref centerY);
                this.TranslateXTo = panXTo;
                this.TranslateYTo = panYTo;
            }
            // manage the center
            this.ScaleCenterX = centerX;
            this.ScaleCenterY = centerY;

            UpdateViewRect();
        }

        private void ApplyPanTransform(double x, double y)
        {
            this.TranslateXFrom = this.TranslateXTo;
            this.TranslateYFrom = this.TranslateYTo;

            double panXTo = this.TranslateXTo + x;
            double panYTo = this.TranslateYTo + y;
            
            // reset the zoom transformation
            this.ScaleXFrom = this.ScaleXTo;
            this.ScaleYFrom = this.ScaleYTo;
            double scaleCenterX = this.ScaleCenterX;
            double scaleCenterY = this.ScaleCenterY;
            this.ApplyPanAdjusments(ref panXTo, ref panYTo, this.ScaleXTo, this.ScaleYTo, ref scaleCenterX, ref scaleCenterY);

            this.TranslateXTo = panXTo;
            this.TranslateYTo = panYTo;

            UpdateViewRect();
        }

        private void ApplyPanAdjusments(ref double panXTo, ref double panYTo, double targetScaleX, double targetScaleY, ref double centerX, ref double centerY)
        {
            // CalculateTransformRect
            double transformWidth = targetScaleX * WindowRect.Width;
            double transformHeight = targetScaleY * WindowRect.Height;

            Rect transformRect = new Rect(
                centerX - transformWidth/2.0, 
                centerY - transformHeight/2.0,
                transformWidth,
                transformHeight);

            //////double transformLeft = WindowRect.Left - (transformWidth - WindowRect.Width) * 0.5;
            //////double transformTop = WindowRect.Top - (transformHeight - WindowRect.Height) * 0.5;

            ////if (transformRect.Left > WindowRect.Left)
            ////{
            ////    centerX -= (transformRect.Left - WindowRect.Left) / 2.0;
            ////    transformRect.X = WindowRect.Left;
            ////}
            ////else if (transformRect.Right < WindowRect.Right)
            ////{
            ////    centerX += (WindowRect.Right - transformRect.Right) / 2.0;
            ////    transformRect.X = WindowRect.Right - WindowRect.Width;
            ////}

            ////if (transformRect.Top > WindowRect.Top)
            ////{
            ////    centerY -= (transformRect.Top - WindowRect.Top) / 2.0;
            ////    transformRect.Y = WindowRect.Top;
            ////}
            ////else if (transformRect.Bottom < WindowRect.Bottom)
            ////{
            ////    centerY += (WindowRect.Bottom - transformRect.Bottom) / 2.0;
            ////    transformRect.Y = WindowRect.Bottom - WindowRect.Height;
            ////}

            Point maxTranslationOffset;
            Point minTranslationOffset;

            maxTranslationOffset.X = WindowRect.Left - transformRect.Left;
            maxTranslationOffset.Y = WindowRect.Top - transformRect.Top;
            minTranslationOffset.X = WindowRect.Right - transformRect.Right;
            minTranslationOffset.Y = WindowRect.Bottom - transformRect.Bottom;

            if (panXTo < minTranslationOffset.X)
            {
                panXTo = minTranslationOffset.X;
            }
            else if (panXTo > maxTranslationOffset.X)
            {
                panXTo = maxTranslationOffset.X;
            }

            if (panYTo < minTranslationOffset.Y)
            {
                panYTo = minTranslationOffset.Y;
            }
            else if (panYTo > maxTranslationOffset.Y)
            {
                panYTo = maxTranslationOffset.Y;
            }
        }

        private void UpdateViewRect()
        {
            Contract.Requires(0 != this.ScaleX, "Invalid scale factor!");
            Contract.Requires(0 != this.ScaleY, "Invalid scale factor!");

            if (!IsZoomed())
            {
                // same as window rect
                _viewRect.X = this.WindowRect.Left;
                _viewRect.Y = this.WindowRect.Top;
                _viewRect.Width = this.WindowRect.Width;
                _viewRect.Height = this.WindowRect.Height;
            }
            else
            {
                // apply current scale factor
                _viewRect.Width = this.WindowRect.Width / this.ScaleX;
                _viewRect.Height = this.WindowRect.Height / this.ScaleY;

                _viewRect.X = (this.WindowRect.Width - _viewRect.Width) / 2.0;
                _viewRect.Y = (this.WindowRect.Height - _viewRect.Height) / 2.0;
            
                // apply current pan
                _viewRect.X -= this.TranslateXTo / this.ScaleX;
                _viewRect.Y -= this.TranslateYTo / this.ScaleY;
            }
        }

        private bool IsZoomed()
        {
            return (MIN_ZOOM_FACTOR != this.ScaleXTo || MIN_ZOOM_FACTOR != this.ScaleYTo);
        }
    }
}
