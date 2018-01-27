using Lykke.SettingsReader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings
{
    /*
     "Assets": 
  {
    "ServiceUrl": "http://assets.lykke-service.svc.cluster.local",
    "CacheExpirationPeriod": "00:05:00"
  },
         */
    public class AssetSettings
    {
        [HttpCheck("/api/isalive")]
        public string ServiceUrl { get; set; }

        public TimeSpan CacheExpirationPeriod { get; set; }
    }
}
