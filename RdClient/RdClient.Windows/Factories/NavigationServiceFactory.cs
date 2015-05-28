namespace RdClient.Factories
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Telemetry;

    class NavigationServiceFactory
    {
        public INavigationService CreateNavigationService(ITelemetryClient telemetryClient)
        {
            IPresentableViewFactory viewFactory = new PresentableViewFactory<PresentableViewConstructor>();
            //
            // Make the connection center a singleton
            //
            viewFactory.AddViewClass("ConnectionCenterView", typeof(Views.ConnectionCenterView), true);
            viewFactory.AddViewClass("AddOrEditDesktopView", typeof(Views.AddOrEditDesktopView));
            viewFactory.AddViewClass("AddUserView", typeof(Views.AddUserView));
            viewFactory.AddViewClass("ErrorMessageView", typeof(Views.ErrorMessageView));
            viewFactory.AddViewClass("DeleteDesktopsView", typeof(Views.DeleteDesktopsView));
            viewFactory.AddViewClass("CertificateValidationView", typeof(Views.CertificateValidationView));
            viewFactory.AddViewClass("DesktopIdentityValidationView", typeof(Views.DesktopIdentityValidationView));
            viewFactory.AddViewClass("SettingsView", typeof(Views.SettingsView));
            viewFactory.AddViewClass("DeleteUserView", typeof(Views.DeleteUserView));
            viewFactory.AddViewClass("AddOrEditWorkspaceView", typeof(Views.AddOrEditWorkspaceView));
            viewFactory.AddViewClass("InSessionEditCredentialsView", typeof(Views.InSessionEditCredentialsView));
            viewFactory.AddViewClass("AddOrEditGatewayView", typeof(Views.AddOrEditGatewayView));
            viewFactory.AddViewClass("DeleteGatewayView", typeof(Views.DeleteGatewayView));
            viewFactory.AddViewClass("SelectNewResourceTypeView", typeof(Views.SelectNewResourceTypeView));
            viewFactory.AddViewClass("DesktopEditorView", typeof(Views.DesktopEditorView));
            viewFactory.AddViewClass("AdditionalToolbarCommandsView", typeof(Views.AdditionalToolbarCommandsView));
            viewFactory.AddViewClass("AboutView", typeof(Views.AboutView));
            viewFactory.AddViewClass("RichTextView", typeof(Views.RichTextView));

            //
            // Remote session view must be a singleton, because it creates the swap chain panel passed
            // to the RDP CX component. The component is designed to use a single swap chain panel.
            //
            viewFactory.AddViewClass("RemoteSessionView", typeof(Views.RemoteSessionView), true);

            DispatchedNavigationService navigationService = new DispatchedNavigationService(telemetryClient);
            navigationService.ViewFactory = viewFactory;

            return navigationService;
        }

        public INavigationExtension CreateDataModelExtension(ApplicationDataModel appDataModel)
        {
            DataModelExtension dataModelExtension = new DataModelExtension()
            {
                AppDataModel = appDataModel
            };

            return dataModelExtension;
        }

        public INavigationExtension CreateDeferredExecutionExtension(IDeferredExecution deferredExecution)
        {
            return new DeferredExecutionExtension() { DeferredExecution = deferredExecution };
        }
    }
}
