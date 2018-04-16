namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validations
{
    public enum ValidationErrorType
    { 
        None,
        AddressIsNotValid,
        FieldNotValid,
        LessThanMinCashout,
        HotwalletTargetProhibited,
        InnerAddressNotFound
    }
}
