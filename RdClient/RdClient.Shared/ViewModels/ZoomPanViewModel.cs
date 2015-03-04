using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Input.Mouse;
    using RdClient.Shared.Input.ZoomPan;
    using Windows.Foundation;

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


    public sealed class ZoomPanViewModel : MutableObject, IZoomPanManipulator
    {
        private IZoomPanTransform _zoomPanTransform;

        private const double MAX_ZOOM_FACTOR = 2.5;
        private const double MIN_ZOOM_FACTOR = 1.0;
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
        public Rect WindowRect { get; set; }

        public IZoomPanTransform ZoomPanTransform
        {
            get { return _zoomPanTransform; }
            private set { this.SetProperty<IZoomPanTransform>(ref _zoomPanTransform, value); }
        }

        public void HandlePanChange(object sender, PanEventArgs e)
        {
            if (null != e)
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
                    double targetXScale = this.ScaleXTo * e.ToLength / e.FromLength;
                    double targetYScale = this.ScaleYTo * e.ToLength / e.FromLength;
                    this.ApplyZoomTransform(e.CenterX, e.CenterY, targetXScale, targetYScale);
                    this.ZoomPanTransform = new CustomZoomTransform(e.CenterX, e.CenterY, targetXScale, targetYScale);
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
                        if(MIN_ZOOM_FACTOR != this.ScaleXTo || MIN_ZOOM_FACTOR != this.ScaleYTo)
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
                        if (MIN_ZOOM_FACTOR != this.ScaleXTo || MIN_ZOOM_FACTOR != this.ScaleYTo)
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

            WindowRect = new Rect(0, 0, 0, 0);
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
            if (null != panTransform)
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

            //
            // Translate (through animation) the swap chain panel to fit the screen
            //
            this.TranslateXTo = WindowRect.X;
            this.TranslateYTo = WindowRect.Y;
        }

        private void ApplyZoomTransform(double centerX, double centerY, double scaleX, double scaleY)
        {
            double targetScaleX = scaleX;
            double targetScaleY = scaleY;
            
            if (targetScaleX < MIN_ZOOM_FACTOR)
            {
                targetScaleX = MIN_ZOOM_FACTOR;
            }
            else if (targetScaleX > MAX_ZOOM_FACTOR)
            {
                targetScaleX = MAX_ZOOM_FACTOR;
            }

            if (targetScaleY < MIN_ZOOM_FACTOR)
            {
                targetScaleY = MIN_ZOOM_FACTOR;
            }
            else if (targetScaleY > MAX_ZOOM_FACTOR)
            {
                targetScaleY = MAX_ZOOM_FACTOR;
            }

            // reset the pan transformation
            this.TranslateXFrom = this.TranslateXTo;
            this.TranslateYFrom = this.TranslateYTo;

            if (this.ScaleXTo > this.ScaleXFrom || this.ScaleYTo > this.ScaleYFrom)
            {
                // shrinking may required pan adjustment
                double panXTo = this.TranslateXTo;
                double panYTo = this.TranslateYTo;
                this.ApplyPanAdjusments(ref panXTo, ref panYTo, targetScaleX, targetScaleY);
                this.TranslateXTo = panXTo;
                this.TranslateYTo = panYTo;
            }

            this.ScaleXFrom = this.ScaleXTo;
            this.ScaleXTo = targetScaleX;
            this.ScaleYFrom = this.ScaleYTo;
            this.ScaleYTo = targetScaleY;

            // manage the center
            this.ScaleCenterX = centerX;
            this.ScaleCenterY = centerY;

            if(MIN_ZOOM_FACTOR == targetScaleX && MIN_ZOOM_FACTOR == targetScaleY)
            {
                this.State = ZoomPanState.PointerMode_DefaultScale;
            }
            else
            {
                this.State = ZoomPanState.PointerMode_Zooming;
            }            
        }

        private void ApplyPanTransform(double x, double y)
        {
            double panXTo = this.TranslateXTo + x;
            double panYTo = this.TranslateYTo + y;

            // reset the zoom transformation
            this.ScaleXFrom = this.ScaleXTo;
            this.ScaleYFrom = this.ScaleYTo;

            this.ApplyPanAdjusments(ref panXTo, ref panYTo, this.ScaleXTo, this.ScaleYTo);

            this.TranslateXFrom = this.TranslateXTo;
            this.TranslateYFrom = this.TranslateYTo;
            this.TranslateXTo = panXTo;
            this.TranslateYTo = panYTo;
        }

        private void ApplyPanAdjusments(ref double panXTo, ref double panYTo, double targetScaleX, double targetScaleY)
        {
            // CalculateTransformRect
            double transformWidth = targetScaleX * WindowRect.Width;
            double transformHeight = targetScaleY * WindowRect.Height;
            double transformLeft = WindowRect.Left - (transformWidth - WindowRect.Width) * 0.5;
            double transformTop = WindowRect.Top - (transformHeight - WindowRect.Height) * 0.5;

            Point maxTranslationOffset;
            Point minTranslationOffset;

            Rect transformRect = new Rect(transformLeft, transformTop, transformWidth, transformHeight);
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
    }
}
