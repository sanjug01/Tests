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
    using RdClient.Shared.Input.ZoomPan;


    public sealed class PanKnobViewModel : MutableObject
    {
        private IZoomPanTransform _zoomPanTransform;

        private double _translateXFrom;
        private double _translateXTo;
        private double _translateYFrom;
        private double _translateYTo;

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
        public IZoomPanTransform ZoomPanTransform
        {
            get { return _zoomPanTransform; }
            private set { this.SetProperty<IZoomPanTransform>(ref _zoomPanTransform, value); }
        }

        private readonly ICommand _panCommand;
        public ICommand PanCommand { get { return _panCommand; } }

        public PanKnobViewModel()
        {
            _panCommand = new RelayCommand(new Action<object>(PanTranslate));


            TranslateXFrom = 0.0;
            TranslateYFrom = 0.0;
            TranslateXTo = 0.0;
            TranslateYTo = 0.0;
        }

        private void PanTranslate(object o)
        {
            IPanTransform panTransform = (o as IPanTransform);
            if (null != panTransform)
            {
                this.ApplyPanTransform(panTransform.X, panTransform.Y);
                this.ZoomPanTransform = new PanTransform(panTransform.X, panTransform.Y);
            }
        }



        private void ApplyPanTransform(double x, double y)
        {
            double panXTo = this.TranslateXTo + x;
            double panYTo = this.TranslateYTo + y;

            // reset the zoom transformation

            ////Point maxTranslationOffset;
            ////Point minTranslationOffset;

            ////maxTranslationOffset.X = WindowRect.Left - TransformRect.Left;
            ////maxTranslationOffset.Y = WindowRect.Top - TransformRect.Top;
            ////minTranslationOffset.X = WindowRect.Right - TransformRect.Right;
            ////minTranslationOffset.Y = WindowRect.Bottom - TransformRect.Bottom;

            ////if (panXTo < minTranslationOffset.X)
            ////{
            ////    panXTo = minTranslationOffset.X;
            ////}
            ////else if (panXTo > maxTranslationOffset.X)
            ////{
            ////    panXTo = maxTranslationOffset.X;
            ////}

            ////if (panYTo < minTranslationOffset.Y)
            ////{
            ////    panYTo = minTranslationOffset.Y;
            ////}
            ////else if (panYTo > maxTranslationOffset.Y)
            ////{
            ////    panYTo = maxTranslationOffset.Y;
            ////}

            this.TranslateXFrom = this.TranslateXTo;
            this.TranslateYFrom = this.TranslateYTo;
            this.TranslateXTo = panXTo;
            this.TranslateYTo = panYTo;
        }
    }
}
