using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lykke.Common.Api.Contract.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Extensions
{
    public static class ModelStateExt
    {
        public static ErrorResponse ToErrorResponse(this ModelStateDictionary modelState)
        {
            var response = new ErrorResponse
            {
                ModelErrors = new Dictionary<string, List<string>>(),
            };

            foreach (var state in modelState)
            {
                var messages = state.Value.Errors
                    .Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage))
                    .Select(e => e.ErrorMessage)
                    .Concat(state.Value.Errors
                        .Where(e => string.IsNullOrWhiteSpace(e.ErrorMessage))
                        .Select(e => e.Exception.Message))
                    .ToList();

                if (messages.Any())
                {
                    response.ModelErrors.Add(state.Key, messages);
                }
            }

            return response;
        }
    }
}
