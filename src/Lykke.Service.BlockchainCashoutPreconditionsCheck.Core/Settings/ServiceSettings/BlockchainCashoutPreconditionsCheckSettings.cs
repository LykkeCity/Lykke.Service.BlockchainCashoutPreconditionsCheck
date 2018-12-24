namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Settings.ServiceSettings
{
    public class BlockchainCashoutPreconditionsCheckSettings
    {
        public DbSettings Db { get; set; }
        public int BlockchainApiTimeoutSeconds { get; set; }
    }
}
