using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Exceptions
{
    public class ErrorResponseException : Exception
    {
        public IDictionary<string, IList<string>> Errors { get; private set; }

        public ErrorResponseException(IDictionary<string, IList<string>> error) : base()
        {
            Errors = error;
        }
    }
}
