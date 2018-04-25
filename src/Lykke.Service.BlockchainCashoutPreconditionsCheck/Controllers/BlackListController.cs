using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Contract;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validation;
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
        private readonly IBlackListService _blackListService;

        public BlackListController(IBlackListService blackListService)
        {
            _blackListService = blackListService;
        }

        /// <summary>
        /// is address black listed
        /// </summary>
        /// <returns></returns>
        [HttpGet("is-blocked/{blockchanType}/{blockckedAddress}")]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("IsBlocked")]
        [ProducesResponseType(typeof(IsBlockedResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> IsBlockedAsync([FromRoute] string blockchanType, [FromRoute] string blockckedAddress)
        {
            var isBlocked = await _blackListService.IsBlockedAsync(blockchanType, blockckedAddress);

            return Ok(new IsBlockedResponse()
            {
                IsBlocked = isBlocked
            });
        }

        /// <summary>
        /// is address black listed
        /// </summary>
        /// <returns></returns>
        [HttpGet("{blockchanType}/{blockckedAddress}")]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("Get")]
        [ProducesResponseType(typeof(BlackListResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetBlackListAsync([FromRoute] string blockchanType, [FromRoute] string blockckedAddress)
        {
            var model = await _blackListService.TryGetAsync(blockchanType, blockckedAddress);

            return Ok(Map(model));
        }

        /// <summary>
        /// Take blocked addresses for specific blockchainType
        /// </summary>
        /// <returns></returns>
        [HttpGet("{blockchanType}")]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("GetAll")]
        [ProducesResponseType(typeof(BlackListEnumerationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAsync([FromRoute] string blockchanType, [FromQuery] int take, [FromQuery] string continuationToken)
        {
            var (models, newToken) = await _blackListService.TryGetAllAsync(blockchanType, take, continuationToken);

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
            BlackListModel model = request != null ? new BlackListModel()
            {
                BlockedAddress = request.BlockedAddress,
                BlockedAddressLowCase = request.BlockedAddress.ToLower(),
                BlockchainType = request.BlockchainType,
                IsCaseSensitive = request.IsCaseSensitive
            } : null;

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
            BlackListModel model = request != null ? new BlackListModel()
            {
                BlockedAddress = request.BlockedAddress,
                BlockedAddressLowCase = request.BlockedAddress.ToLower(),
                BlockchainType = request.BlockchainType,
                IsCaseSensitive = request.IsCaseSensitive
            } : null;

            await _blackListService.SaveAsync(model);

            return Ok();
        }

        /// <summary>
        /// Delete black listed address
        /// </summary>
        /// <returns></returns>
        [HttpDelete("/{blockchanType}/{blockckedAddress}")]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("Delete")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteBlockedAddressAsync([FromRoute] string blockchanType, [FromRoute] string blockckedAddress)
        {
            await _blackListService.DeleteAsync(blockchanType, blockckedAddress);

            return Ok();
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
