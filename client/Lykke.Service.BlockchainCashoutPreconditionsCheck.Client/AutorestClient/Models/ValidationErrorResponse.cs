// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.AutorestClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class ValidationErrorResponse
    {
        /// <summary>
        /// Initializes a new instance of the ValidationErrorResponse class.
        /// </summary>
        public ValidationErrorResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ValidationErrorResponse class.
        /// </summary>
        /// <param name="type">Possible values include: 'None',
        /// 'AddressIsNotValid', 'FieldNotValid', 'LessThanMinCashout',
        /// 'HotwalletTargetProhibited', 'DepositAddressNotFound',
        /// 'BlackListedAddress'</param>
        public ValidationErrorResponse(ValidationErrorType type, string value = default(string))
        {
            Type = type;
            Value = value;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets possible values include: 'None', 'AddressIsNotValid',
        /// 'FieldNotValid', 'LessThanMinCashout', 'HotwalletTargetProhibited',
        /// 'DepositAddressNotFound', 'BlackListedAddress'
        /// </summary>
        [JsonProperty(PropertyName = "Type")]
        public ValidationErrorType Type { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Value")]
        public string Value { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
        }
    }
}
