using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Exceptions;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Services;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Filter;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Requests;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Controllers
{
    [Route("api/[controller]")]
    public class BlackListController : Controller
    {
        private static readonly char[] _charactersToTrim = new char[] {' ', '\t'};
        private readonly IBlackListService _blackListService;

        public BlackListController(IBlackListService blackListService)
        {
            _blackListService = blackListService;
        }

        /// <summary>
        /// is address black listed
        /// </summary>
        /// <returns></returns>
        [HttpGet("{blockchainType}/{blockedAddress}/is-blocked")]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("IsBlocked")]
        [ProducesResponseType(typeof(IsBlockedResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> IsBlockedAsync([FromRoute][Required] string blockchainType, [FromRoute][Required] string blockedAddress)
        {
            var isBlocked = await _blackListService.IsBlockedAsync(blockchainType, Trim(blockedAddress));

            return Ok(new IsBlockedResponse()
            {
                IsBlocked = isBlocked
            });
        }

        /// <summary>
        /// is address black listed
        /// </summary>
        /// <returns></returns>
        [HttpGet("{blockchainType}/{blockedAddress}")]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("Get")]
        [ProducesResponseType(typeof(BlackListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetBlackListAsync([FromRoute][Required] string blockchainType, [FromRoute][Required] string blockedAddress)
        {
            var model = await _blackListService.TryGetAsync(blockchainType, Trim(blockedAddress));

            if (model == null)
                return NoContent();

            return Ok(Map(model));
        }

        /// <summary>
        /// Take blocked addresses for specific blockchainType
        /// </summary>
        /// <returns></returns>
        [HttpGet("{blockchainType}")]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("GetAll")]
        [ProducesResponseType(typeof(BlackListEnumerationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync([FromRoute][Required] string blockchainType, [FromQuery] int take, [FromQuery] string continuationToken)
        {
            if (take <= 0)
                throw new ArgumentValidationException("Field must be equal or greater than 0", $"{nameof(take)}");

            var (models, newToken) = await _blackListService.TryGetAllAsync(blockchainType, take, continuationToken);

            return Ok(new BlackListEnumerationResponse()
            {
                ContinuationToken = newToken,
                List = models.Select(x => Map(x))
            });
        }

        /// <summary>
        /// Add black listed address
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("Add")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddBlockedAddressAsync([FromBody] AddBlackListModel request)
        {
            string blockedAddress = Trim(request.BlockedAddress);

            BlackListModel model = new BlackListModel()
            {
                BlockedAddress = blockedAddress,
                BlockedAddressLowCase = blockedAddress.ToLower(),
                BlockchainType = request.BlockchainType,
                IsCaseSensitive = request.IsCaseSensitive
            };

             await _blackListService.SaveAsync(model);

            return Ok();
        }

        /// <summary>
        /// Add black listed address
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("Update")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateBlockedAddressAsync([FromBody] AddBlackListModel request)
        {
            string blockedAddress = Trim(request.BlockedAddress);

            BlackListModel model = new BlackListModel()
            {
                BlockedAddress = blockedAddress,
                BlockedAddressLowCase = blockedAddress.ToLower(),
                BlockchainType = request.BlockchainType,
                IsCaseSensitive = request.IsCaseSensitive
            };

            await _blackListService.SaveAsync(model);

            return Ok();
        }

        /// <summary>
        /// Delete black listed address
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{blockchainType}/{blockedAddress}")]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("Delete")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteBlockedAddressAsync([FromRoute][Required] string blockchainType, [FromRoute][Required] string blockedAddress)
        {
            await _blackListService.DeleteAsync(blockchainType, Trim(blockedAddress));

            return Ok();
        }

        private string Trim(string address)
        {
            return address?.Trim(_charactersToTrim);
        }

        private BlackListResponse Map(BlackListModel blackListModel)
        {
            return new BlackListResponse()
            {
                IsCaseSensitive = blackListModel.IsCaseSensitive,
                BlockedAddress = blackListModel.BlockedAddress,
                BlockchainType = blackListModel.BlockchainType
            };
        }
    }
}
