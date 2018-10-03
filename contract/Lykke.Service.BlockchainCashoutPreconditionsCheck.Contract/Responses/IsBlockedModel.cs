using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Responses
{
    [DataContract]
    public class IsBlockedResponse
    {
        [DataMember]
        public bool IsBlocked { get; set; }
    }
}
