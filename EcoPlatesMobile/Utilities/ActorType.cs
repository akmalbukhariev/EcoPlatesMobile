using System.Text.Json.Serialization;

namespace EcoPlatesMobile.Utilities
{ 
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ActorType
    {
        COMPANY,
        USER,
    }

    public static class ActorTypeExtensions
    {
        public static string GetValue(this ActorType type)
        {
            return type.ToString();
        }

        public static ActorType FromValue(string value)
        {
            if (Enum.TryParse<ActorType>(value, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Unknown ActorType value: {value}");
        }
    }
}