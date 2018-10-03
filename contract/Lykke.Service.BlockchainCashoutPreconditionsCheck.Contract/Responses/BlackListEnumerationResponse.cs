using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Responses
{
    public class BlackListEnumerationResponse
    {
        [DataMember]
        public IEnumerable<BlackListResponse> List { get; set; }

        [DataMember]
        public string ContinuationToken { get; set; }
    }
}
