﻿using RdClient.Navigation;
using RdClient.Shared.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RdClient.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DialogMessage : Page, IPresentableView
    {
        public IViewModel ViewModel { get { return (IViewModel)this.DataContext; } }

        public DialogMessage()
        {
            this.InitializeComponent();
        }

        public void Activating(object activationParameter)
        {
            
        }

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            (this.DataContext as DialogMessageViewModel).DialogView = this;
        }

        public void Dismissing()
        {        
        }
    }
}
