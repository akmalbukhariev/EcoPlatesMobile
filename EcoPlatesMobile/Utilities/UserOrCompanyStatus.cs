using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Utilities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserOrCompanyStatus
    {
        ACTIVE,
        INACTIVE,
        BANNED
    }

    public static class UserOrCompanyStatusExtensions
    {
        public static string GetValue(this UserOrCompanyStatus status)
        {
            return status.ToString();
        }

        public static UserOrCompanyStatus FromValue(string value)
        {
            if (Enum.TryParse<UserOrCompanyStatus>(value, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Unknown UserOrCompanyStatus value: {value}");
        }
    }
}
