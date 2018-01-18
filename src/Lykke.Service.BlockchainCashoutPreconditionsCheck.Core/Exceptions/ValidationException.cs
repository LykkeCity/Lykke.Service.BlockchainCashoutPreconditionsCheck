using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Exceptions
{
    public class ArgumentValidationException : ArgumentException
    {
        public ArgumentValidationException(string message, string paramName) : base(message, paramName)
        {}
    }
}
