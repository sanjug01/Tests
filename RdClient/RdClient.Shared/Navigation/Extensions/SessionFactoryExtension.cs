namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System.Diagnostics.Contracts;

    public sealed class SessionFactoryExtension : MutableObject, INavigationExtension
    {
        private ISessionFactory _sessionFactory;

        /// <summary>
        /// Session factory that the extension passes to view models implementing the
        /// ISessionFactorySite interface.
        /// The property makes the extension XAML-friendly.
        /// </summary>
        public ISessionFactory SessionFactory
        {
            get { return _sessionFactory; }
            set { this.SetProperty(ref _sessionFactory, value); }
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            Contract.Assert(null != _sessionFactory, "Session factory wasn't set for the navigation extension");
            viewModel.CastAndCall<ISessionFactorySite>(site => site.SetSessionFactory(_sessionFactory));
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<ISessionFactorySite>(site => site.SetSessionFactory(null));
        }
    }
}
