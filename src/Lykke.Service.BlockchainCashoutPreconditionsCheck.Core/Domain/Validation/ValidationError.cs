namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Core.Domain.Validations
{
    public class ValidationError
    {
        public ValidationErrorType Type { get; private set; }
        public string Value { get; private set; }

        public static ValidationError Create(ValidationErrorType type, string value)
        {
            return new ValidationError
            {
                Type = type,
                Value = value
            };
        }
    }
}
