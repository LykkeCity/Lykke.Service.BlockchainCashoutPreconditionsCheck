using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Requests
{
    [DataContract]
    public class AddBlackListModel
    {
        [Required]
        [DataMember]
        public string BlockchainType { get; set; }

        [Required]
        [DataMember]
        public string BlockedAddress { get; set; }

        [Required]
        [DataMember]
        public bool IsCaseSensitive { get; set; }
    }
}
