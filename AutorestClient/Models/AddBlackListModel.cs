// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.AutorestClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class AddBlackListModel
    {
        /// <summary>
        /// Initializes a new instance of the AddBlackListModel class.
        /// </summary>
        public AddBlackListModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AddBlackListModel class.
        /// </summary>
        public AddBlackListModel(bool isCaseSensitive, string blockchainType = default(string), string blockedAddress = default(string))
        {
            BlockchainType = blockchainType;
            BlockedAddress = blockedAddress;
            IsCaseSensitive = isCaseSensitive;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "BlockchainType")]
        public string BlockchainType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "BlockedAddress")]
        public string BlockedAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IsCaseSensitive")]
        public bool IsCaseSensitive { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            //Nothing to validate
        }
    }
}
