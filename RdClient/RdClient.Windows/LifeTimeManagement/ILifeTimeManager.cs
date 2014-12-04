
using System.Threading.Tasks;
namespace RdClient.LifeTimeManagement
{
    public interface ILifeTimeManager
    {
        void OnLaunched(IActivationArgs e);
        void OnSuspending(object sender, ISuspensionArgs e);
    }
}
