using Lykke.SettingsReader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings
{
    public class BlockchainSettings
    {
        public string Type { get; set; }

        [HttpCheck("/api/isalive")]
        public string ApiUrl { get; set; }
    }
}
