using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Requests
{
    [DataContract]
    public class CheckCashoutValidityModel
    {
        [Required]
        [DataMember]
        public string AssetId { get; set; }

        [Required]
        [DataMember]
        public decimal Amount { get; set; }

        [Required]
        [DataMember]
        public string DestinationAddress { get; set; }
        
        [DataMember]
        public Guid? ClientId { get; set; }
    }
}
