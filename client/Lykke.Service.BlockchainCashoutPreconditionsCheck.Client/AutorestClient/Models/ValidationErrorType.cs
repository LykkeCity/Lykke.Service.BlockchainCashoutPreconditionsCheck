﻿// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.BlockchainCashoutPreconditionsCheck.Client.AutorestClient.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for ValidationErrorType.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ValidationErrorType
    {
        [EnumMember(Value = "None")]
        None,
        [EnumMember(Value = "AddressIsNotValid")]
        AddressIsNotValid,
        [EnumMember(Value = "FieldNotValid")]
        FieldNotValid,
        [EnumMember(Value = "LessThanMinCashout")]
        LessThanMinCashout,
        [EnumMember(Value = "HotwalletTargetProhibited")]
        HotwalletTargetProhibited,
        [EnumMember(Value = "DepositAddressNotFound")]
        DepositAddressNotFound,
        [EnumMember(Value = "BlackListedAddress")]
        BlackListedAddress
    }
    internal static class ValidationErrorTypeEnumExtension
    {
        internal static string ToSerializedValue(this ValidationErrorType? value)
        {
            return value == null ? null : ((ValidationErrorType)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this ValidationErrorType value)
        {
            switch( value )
            {
                case ValidationErrorType.None:
                    return "None";
                case ValidationErrorType.AddressIsNotValid:
                    return "AddressIsNotValid";
                case ValidationErrorType.FieldNotValid:
                    return "FieldNotValid";
                case ValidationErrorType.LessThanMinCashout:
                    return "LessThanMinCashout";
                case ValidationErrorType.HotwalletTargetProhibited:
                    return "HotwalletTargetProhibited";
                case ValidationErrorType.BlackListedAddress:
                    return "BlackListedAddress";
                case ValidationErrorType.DepositAddressNotFound:
                    return "DepositAddressNotFound";
            }
            return null;
        }

        internal static ValidationErrorType? ParseValidationErrorType(this string value)
        {
            switch( value )
            {
                case "None":
                    return ValidationErrorType.None;
                case "AddressIsNotValid":
                    return ValidationErrorType.AddressIsNotValid;
                case "FieldNotValid":
                    return ValidationErrorType.FieldNotValid;
                case "LessThanMinCashout":
                    return ValidationErrorType.LessThanMinCashout;
                case "HotwalletTargetProhibited":
                    return ValidationErrorType.HotwalletTargetProhibited;
                case "BlackListedAddress":
                    return ValidationErrorType.BlackListedAddress;
                case "DepositAddressNotFound":
                    return ValidationErrorType.DepositAddressNotFound;
            }
            return null;
        }
    }
}
