using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Responses
{
    public class BlackListResponse
    {
        [DataMember]
        public string BlockchainType { get; set; }

        [DataMember]
        public string BlockedAddress { get; set; }

        [DataMember]
        public bool IsCaseSensitive { get; set; }
    }

    public class BlackListEnumerationResponse
    {
        [DataMember]
        public IEnumerable<BlackListResponse> List { get; set; }

        [DataMember]
        public string ContinuationToken { get; set; }
    }
}
