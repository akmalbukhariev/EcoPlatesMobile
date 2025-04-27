
namespace EcoPlatesMobile.Utilities
{ 
    public enum PromotionFeedbackType
    {
        NONE,
        GREAT_VALUE,
        DELICIOUS_FOOD,
        GREAT_SERVICE
    }

    public static class PromotionFeedbackTypeExtensions
    {
        public static string GetValue(this PromotionFeedbackType type)
        {
            return type.ToString();
        }

        public static PromotionFeedbackType FromValue(string value)
        {
            if (Enum.TryParse<PromotionFeedbackType>(value, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Unknown PromotionFeedbackType value: {value}");
        }
    }
}
