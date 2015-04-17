﻿namespace RdClient.Controls
{
    using RdClient.Shared.Input.ZoomPan;
    using RdClient.Shared.ViewModels;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Windows.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public sealed partial class ConnectionBarControl : UserControl
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items",
            typeof(ObservableCollection<object>), typeof(ConnectionBarControl),
            new PropertyMetadata(null, OnItemsChanged));

        public static readonly DependencyProperty MainActionProperty = DependencyProperty.Register("MainAction",
            typeof(ICommand), typeof(ConnectionBarControl),
            new PropertyMetadata(null, OnMainActionChanged));

        public static readonly DependencyProperty ZoomPanStateProperty = DependencyProperty.Register("ZoomPanState",
            typeof(ZoomPanState), typeof(ConnectionBarControl),
            new PropertyMetadata(ZoomPanState.PointerMode_DefaultScale, OnZoomPanStateChanged));

        public static readonly DependencyProperty ToggleZoomProperty = DependencyProperty.Register("ToggleZoom",
            typeof(ICommand), typeof(ConnectionBarControl),
            new PropertyMetadata(null, OnToggleZoomChanged));


        public static readonly DependencyProperty IsKeyboardVisibleProperty = DependencyProperty.Register("IsKeyboardVisible",
            typeof(bool), typeof(ConnectionBarControl),
            new PropertyMetadata(null, OnIsKeyboardVisibleChanged));

        public ObservableCollection<object> Items
        {
            get { return (ObservableCollection<object>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public ICommand MainAction
        {
            get { return (ICommand)GetValue(MainActionProperty); }
            set { SetValue(MainActionProperty, value); }
        }

        public ZoomPanState ZoomPanState
        {
            get { return (ZoomPanState)GetValue(ZoomPanStateProperty); }
            set { SetValue(ZoomPanStateProperty, value); }
        }

        public ICommand ToggleZoom
        {
            get { return (ICommand)GetValue(ToggleZoomProperty); }
            set { SetValue(ToggleZoomProperty, value); }
        }

        public bool IsKeyboardVisible
        {
            get { return (bool)GetValue(IsKeyboardVisibleProperty); }
            set { SetValue(IsKeyboardVisibleProperty, value); }
        }

        public ConnectionBarControl()
        {
            this.InitializeComponent();
            this.Items = new ObservableCollection<object>();
        }

        private static void OnMainActionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ConnectionBarControl)sender).InternalOnMainActionChanged(e);
        }

        private void InternalOnMainActionChanged(DependencyPropertyChangedEventArgs e)
        {
            this.MainActionButton.Command = (ICommand)e.NewValue;
        }

        private static void OnZoomPanStateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ConnectionBarControl)sender).InternalOnZoomPanStateChanged(e);
        }

        private void InternalOnZoomPanStateChanged(DependencyPropertyChangedEventArgs e)
        {
            ZoomPanState newState = (ZoomPanState)e.NewValue;
            Visibility
                zoomIn = Visibility.Visible,
                zoomOut = Visibility.Visible;

            switch(newState)
            {
                case ZoomPanState.TouchMode_MinScale:
                    zoomOut = Windows.UI.Xaml.Visibility.Collapsed;
                    break;

                case ZoomPanState.TouchMode_MaxScale:
                    zoomIn = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
            }

            this.ZoomInButton.Visibility = zoomIn;
            this.ZoomOutButton.Visibility = zoomOut;
        }

        private static void OnItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ConnectionBarControl)sender).InternalOnItemsChanged(e);
        }

        private void InternalOnItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            ObservableCollection<object> newCollection = (ObservableCollection<object>)e.NewValue;

            this.ItemsControl.Items.Clear();
            foreach (object item in newCollection)
                this.ItemsControl.Items.Add(item);

            newCollection.CollectionChanged += (sender, args) =>
            {
                this.ItemsControl.Items.Clear();
                foreach (object item in newCollection)
                    this.ItemsControl.Items.Add(item);
            };
        }

        private static void OnToggleZoomChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ConnectionBarControl)sender).InternalOnToggleZoomChanged(e);
        }

        private void InternalOnToggleZoomChanged(DependencyPropertyChangedEventArgs e)
        {
            this.ZoomInButton.Command = (ICommand)e.NewValue;
            this.ZoomOutButton.Command = (ICommand)e.NewValue;
        }

        private static void OnIsKeyboardVisibleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ConnectionBarControl)sender).InternalOnIsKeyboardVisibleChanged(e);
        }

        private void InternalOnIsKeyboardVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.KeyboardButton.IsChecked = (bool)e.NewValue;
        }
    }
}
