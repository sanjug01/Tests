namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Telemetry;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Input;
    using Windows.ApplicationModel;
    using Windows.UI.Xaml;

    public sealed class AboutViewModel : AccessoryViewModelBase, ITelemetryClientSite
    {
        private string _appVersion;
        private string _copyright;
        private readonly ICommand _closeCommand;
        private readonly ICommand _showEulaCommand;
        private readonly ICommand _showThirdPartyDocCommand;
        private readonly ICommand _showPrivacyCommand;
        private ITelemetryClient _telemetryClient;

        public AboutViewModel()
        {
            _closeCommand = new RelayCommand(o => { this.DismissModal(null); });
            _showEulaCommand = new RelayCommand(ShowEulaExecute);
            _showPrivacyCommand = new RelayCommand(ShowPrivacyDocExecute);
            _showThirdPartyDocCommand = new RelayCommand(ShowThirdPartyDocExecute);
        }

        public string AppVersion
        {
            get
            {
                if (null == _appVersion)
                {
                    PackageVersion pv = Package.Current.Id.Version;
                    _appVersion = string.Format("{0}.{1}.{2}.{3}", pv.Major, pv.Minor, pv.Revision, pv.Build);
                }

                return _appVersion;
            }
        }

        public string AppName
        {
            get { return Package.Current.Description; }
        }

        public string Copyright
        {
            get
            {
                if(null == _copyright)
                {
                    Assembly appAsm = Application.Current.GetType().GetTypeInfo().Assembly;
                    AssemblyCopyrightAttribute attr = GetAssemblyAttribute<AssemblyCopyrightAttribute>(appAsm);

                    if(null != attr)
                        _copyright = attr.Copyright;
                }

                return _copyright;
            }
        }

        public ICommand Close { get { return _closeCommand; } }
        public ICommand ShowEulaCommand { get { return _showEulaCommand; } }
        public ICommand ShowPrivacyCommand { get { return _showPrivacyCommand; } }
        public ICommand ShowThirdPartyNoticesCommand { get { return _showThirdPartyDocCommand; } }

        protected override void DefaultAction()
        {
            DismissModal(null);
        }
        
        private static TAttr GetAssemblyAttribute<TAttr>(Assembly assembly) where TAttr : Attribute
        {
            //
            // Extract the first custom attribute of the generic type from the assembly.
            //
            TAttr rt = null;
            IEnumerable<Attribute> attrs = assembly.GetCustomAttributes(typeof(TAttr));
            IEnumerator<Attribute> enattr = attrs.GetEnumerator();

            if (enattr.MoveNext())
                rt = enattr.Current as TAttr;

            return rt;
        }

        private void ShowEulaExecute(object o)
        {
            RichTextViewModelArgs args = new RichTextViewModelArgs(InternalDocType.EulaDoc);
            NavigationService.PushAccessoryView("RichTextView", args);
            _telemetryClient.ReportEvent(new Telemetry.Events.ViewedLicense());
        }

        private void ShowThirdPartyDocExecute(object o)
        {
            RichTextViewModelArgs args = new RichTextViewModelArgs(InternalDocType.ThirdPartyNotices);
            NavigationService.PushAccessoryView("RichTextView", args);
            _telemetryClient.ReportEvent(new Telemetry.Events.ViewedThirdPartyDoc());
        }

        private void ShowPrivacyDocExecute(object o)
        {
            _telemetryClient.ReportEvent(new Telemetry.Events.ViewedPrivacy());
        }

        void ITelemetryClientSite.SetTelemetryClient(ITelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }
    }
}
