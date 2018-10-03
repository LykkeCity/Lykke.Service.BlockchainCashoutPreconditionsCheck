using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Responses
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
}
