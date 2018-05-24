using System;
using System.Collections.Generic;
using System.Text;
using Lykke.Service.BlockchainCashoutPreconditionsCheck.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Filter
{
    public class CheckModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                context.Result = new BadRequestObjectResult(context.ModelState.ToErrorResponse());
            }

            base.OnActionExecuting(context);
        }
    }
}
