using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.AutorestClient.Models;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client
{
    public interface IBlockchainCashoutPreconditionsCheckClient
    {
        Task<(bool isAllowed, IEnumerable<ValidationErrorResponse>)> ValidateCashoutAsync(CashoutValidateModel validateCashoutModel);
    }
}
