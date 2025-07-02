using System.Globalization;
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Converters
{
    public class FeedbackTypeToLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string feedbackType)
            {
                var type = PromotionFeedbackTypeExtensions.FromValue(feedbackType);

                switch (type)
                {
                    case PromotionFeedbackType.GREAT_VALUE:
                        return AppResource.GreatValue;
                    case PromotionFeedbackType.DELICIOUS_FOOD:
                        return AppResource.DeliciousFood;
                    case PromotionFeedbackType.GREAT_SERVICE:
                        return AppResource.GreatService;
                    default:
                        return "Unknown";
                }
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
