namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validations
{
    public enum ValidationErrorType
    {
        None,
        AddressIsNotValid,
        FieldIsNotValid,
        LessThanMinCashout,
        HotwalletTargetProhibited,
        BlackListedAddress,
        DepositAddressNotFound,
        CashoutToSelfAddress,
        Error
    }
}
