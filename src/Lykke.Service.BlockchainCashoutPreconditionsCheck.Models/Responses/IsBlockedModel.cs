using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Models.Responses
{
    [DataContract]
    public class IsBlockedResponse
    {
        [DataMember]
        public bool IsBlocked { get; set; }
    }
}
