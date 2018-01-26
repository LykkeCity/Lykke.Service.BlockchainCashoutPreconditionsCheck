using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Filter
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ArgumentValidationExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        public ArgumentValidationExceptionFilterAttribute()
        {

        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ArgumentValidationException argException)
            {
                ErrorResponse error= ErrorResponse.Create("Validation Error");

                error.AddModelError("exception", argException);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(error);
                byte[] data = Encoding.UTF8.GetBytes(jsonString);
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.Body.Write(data, 0, data.Length);
                context.ExceptionHandled = true;
            }
        }
    }
}
