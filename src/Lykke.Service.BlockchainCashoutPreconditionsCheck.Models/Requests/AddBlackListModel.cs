using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Requests
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
