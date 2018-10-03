using System;
using System.Collections.Generic;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Exceptions
{
    public class ErrorResponseException : Exception
    {
        public IDictionary<string, List<string>> Errors { get; private set; }

        public ErrorResponseException(IDictionary<string, List<string>> error) : base()
        {
            Errors = error;
        }
    }
}
