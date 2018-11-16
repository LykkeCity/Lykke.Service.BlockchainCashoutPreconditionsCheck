using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validations;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services
{
    public interface IValidationService
    {
        Task<IReadOnlyCollection<ValidationError>> ValidateAsync(CashoutModel cashoutModel);
    }
}
