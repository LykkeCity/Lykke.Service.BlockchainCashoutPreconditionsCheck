using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings
{
    /*
        "BlockchainsIntegration": {
          "Blockchains": [
            {
              "Type": "LiteCoin",
              "ApiUrl": "http://litecoin-api.lykke-service.svc.cluster.local",
              "SignFacadeUrl": "http://blockchain-sign-service-litecoin.lykke-service.svc.cluster.local",
              "HotWalletAddress": "lc-123",
              "Monitoring": {
                "InProgressOperationAlarmPeriod": "00:01:00"
              }
            },
            {
              "Type": "EthereumClassic",
              "ApiUrl": "http://ethereum-classic-api.lykke-service.svc.cluster.local",
              "SignFacadeUrl": "http://blockchain-sign-service-ethereum-classic.lykke-service.svc.cluster.local",
              "HotWalletAddress": "lc-123",
              "Monitoring": {
                "InProgressOperationAlarmPeriod": "00:01:00"
              }
            }
          ]
        },
     */

    public class BlockchainsIntegrationSettings
    {
        public IEnumerable<BlockchainSettings> Blockchains { get; set; }
    }

    public class BlockchainSettings
    {
        public string Type { get; set; }

        public string ApiUrl { get; set; }
    }
}
