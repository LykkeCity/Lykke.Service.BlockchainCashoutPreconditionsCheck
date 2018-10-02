using System.Threading.Tasks;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Requests;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Refit;
using AddBlackListModel = Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Requests.AddBlackListModel;
using IsAliveResponse = Lykke.Common.Api.Contract.Responses.IsAliveResponse;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.ClientGenerator
{
    public interface IBlockchainCashoutPreconditionsCheckApi
    {
        [Put("/api/BlackList")]
        Task UpdateBlackListAsync(AddBlackListModel updateModel);

        [Post("/api/BlackList")]
        Task CreateBlackListAsync(AddBlackListModel createModel);

        [Delete("/api/BlackList/{blockchainType}/{blockedAddress}")]
        Task DeleteBlackListAsync(string blockchainType, string blockedAddress);

        [Get("/api/BlackList/{blockchainType}/{blockedAddress}")]
        Task<BlackListResponse> GetBlackListAsync(string blockchainType, string blockedAddress);

        [Get("/api/BlackList/{blockchainType}")]
        Task<BlackListEnumerationResponse> GetBlackListsAsync(string blockchainType, 
            [FromQuery]int take, 
            [FromQuery] string continuationToken);

        [Get("/api/BlackList/{blockchainType}/{blockedAddress}/is-blocked")]
        Task<IsBlockedResponse> IsBlockedAsync(string blockchainType, string blockedAddress);

        [Get("/api/CashoutCheck")]
        Task<CashoutValidityResult> CashoutCheckAsync(CheckCashoutValidityModel cashoutValidityModel);

        [Get("/api/IsAlive")]
        Task<IsAliveResponse> GetIsAliveAsync();
    }
}
