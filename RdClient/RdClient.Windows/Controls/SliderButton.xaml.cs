using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace RdClient.Controls
{
    public sealed partial class SliderButton : UserControl
    {

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(Double),
            typeof(SliderButton),
            new PropertyMetadata(null, OnValueChanged));

        public Double Value
        {
            get
            {
                return (Double)this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
            }
        }

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SliderButton)sender).OnValueChanged((Double) e.OldValue, (Double) e.NewValue);
        }

        private void OnValueChanged(Double oldValue, Double newValue)
        {
            this.Slider.Value = newValue;
        }

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle",
            typeof(Style),
            typeof(SliderButton),
            new PropertyMetadata(null, OnButtonStyleChanged));

        public Style ButtonStyle
        {
            get { return (Style)this.GetValue(ButtonStyleProperty); }
            set { this.SetValue(ButtonStyleProperty, value); }
        }

        private static void OnButtonStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SliderButton)sender).OnButtonStyleChanged((Style) e.OldValue, (Style) e.NewValue);
        }

        private void OnButtonStyleChanged(Style oldValue, Style newValue)
        {
            this.Button.Style = newValue;
        }

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText",
            typeof(String),
            typeof(SliderButton),
            new PropertyMetadata(null, OnButtonTextChanged));

        public String ButtonText
        {
            get { return (String)this.GetValue(ButtonTextProperty); }
            set { this.SetValue(ButtonTextProperty, value); }
        }

        private static void OnButtonTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((SliderButton)sender).OnButtonTextChanged((String)e.OldValue, (String)e.NewValue);
        }

        private void OnButtonTextChanged(String oldValue, String newValue)
        {
            this.Button.Content = newValue;
        }

        public SliderButton()
        {
            this.InitializeComponent();
            this.Slider.ValueChanged += OnSliderValueChanged;
        }

        private void OnSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            this.Value = e.NewValue;
        }
    }
}
