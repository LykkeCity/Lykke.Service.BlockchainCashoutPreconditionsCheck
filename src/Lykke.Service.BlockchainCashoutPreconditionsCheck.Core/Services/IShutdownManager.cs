using System.Threading.Tasks;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}