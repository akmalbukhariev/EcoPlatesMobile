using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Utilities
{ 
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FeedbackType
    {
        SUGGESTIONS,
        COMPLAINTS,
    }

    public static class PosterTypeExtensions
    {
        public static string GetValue(this FeedbackType type)
        {
            return type.ToString();
        }

        public static FeedbackType FromValue(string value)
        {
            if (Enum.TryParse<FeedbackType>(value, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Unknown FeedbackType value: {value}");
        }
    }
}
