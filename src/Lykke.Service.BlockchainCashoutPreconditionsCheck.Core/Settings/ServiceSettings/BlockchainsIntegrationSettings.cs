using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings
{
    public class BlockchainsIntegrationSettings
    {
        public IEnumerable<BlockchainSettings> Blockchains { get; set; }
    }
}
