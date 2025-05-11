
namespace EcoPlatesMobile.Utilities
{ 
    public enum PosterFeedbackType
    {
        NONE,
        GREAT_VALUE,
        DELICIOUS_FOOD,
        GREAT_SERVICE
    }

    public static class PosterFeedbackTypeExtensions
    {
        public static string GetValue(this PosterFeedbackType type)
        {
            return type.ToString();
        }

        public static PosterFeedbackType FromValue(string value)
        {
            if (Enum.TryParse<PosterFeedbackType>(value, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Unknown PosterFeedbackType value: {value}");
        }
    }
}
