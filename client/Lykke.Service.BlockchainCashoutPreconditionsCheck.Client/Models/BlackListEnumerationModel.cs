using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.Models
{
    public class BlackListEnumerationModel
    {
        public BlackListEnumerationModel()
        {}

        public IEnumerable<BlackListModel> List { get; set; }

        public string ContinuationToken { get; set; }
    }
}
