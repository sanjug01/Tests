namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Navigation;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Input;
    using Windows.ApplicationModel;
    using Windows.UI.Xaml;

    public sealed class AboutViewModel : ViewModelBase
    {
        private readonly ICommand _navigateBack;
        private string _appVersion;
        private string _copyright;

        public AboutViewModel()
        {
            _navigateBack = new RelayCommand(o => this.DismissModal(null));
        }

        public ICommand NavigateBack
        {
            get { return _navigateBack; }
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

        protected override void OnNavigatingBack(IBackCommandArgs backArgs)
        {
            base.OnNavigatingBack(backArgs);
            DismissModal(null);
        }

        private static T GetAssemblyAttribute<T>( Assembly assembly ) where T : Attribute
        {
            T rt = null;
            IEnumerable<Attribute> attrs = assembly.GetCustomAttributes(typeof(T));
            IEnumerator<Attribute> enattr = attrs.GetEnumerator();

            if (enattr.MoveNext())
                rt = enattr.Current as T;

            return rt;
        }

    }
}
