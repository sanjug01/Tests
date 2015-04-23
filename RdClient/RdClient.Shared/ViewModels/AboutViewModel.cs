namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Windows.ApplicationModel;
    using Windows.UI.Xaml;

    public sealed class AboutViewModel : AccessoryViewModelBase
    {
        private string _appVersion;
        private string _copyright;

        public AboutViewModel()
        {
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
            get { return Package.Current.DisplayName; }
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
    }
}
