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
    public class CashoutCheckController : Controller
    {
        private readonly IValidationService _validationService;

        public CashoutCheckController(IValidationService validationService)
        {
            _validationService = validationService;
        }

        /// <summary>
        /// Checks service is alive
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ArgumentValidationExceptionFilter]
        [SwaggerOperation("Check")]
        [ProducesResponseType(typeof(CashoutValidityResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ValidateAsync(CheckCashoutValidityModel model)
        {
            CashoutModel cashoutModel = model != null ? new CashoutModel()
            {
                AssetId = model.AssetId,
                DestinationAddress = model.DestinationAddress,
                Volume = model.Amount,
                ClientId = model.ClientId
            } : null;

            var validationErrors = await _validationService.ValidateAsync(cashoutModel);

            var response = new CashoutValidityResult()
            {
                IsAllowed = (validationErrors?.Count() ?? 0) == 0,
                ValidationErrors = validationErrors.Select(x => ValidationErrorResponse.Create((ValidationErrorType)x.Type, x.Value)),
            };

            return Ok(response);
        }
    }
}
