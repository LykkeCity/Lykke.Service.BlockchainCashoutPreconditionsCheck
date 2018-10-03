using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Contract.Requests
{
    [DataContract]
    public class CheckCashoutValidityModel
    {
        [Required]
        [DataMember]
        public string AssetId { get; set; }

        [DataMember]
        public decimal? Amount { get; set; }

        [Required]
        [DataMember]
        public string DestinationAddress { get; set; }
        
        [DataMember]
        public Guid? ClientId { get; set; }
    }
}
