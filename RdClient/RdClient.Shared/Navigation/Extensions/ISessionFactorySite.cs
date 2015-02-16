namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Models;

    public interface ISessionFactorySite
    {
        void SetSessionFactory(ISessionFactory sessionFactory);
    }
}
