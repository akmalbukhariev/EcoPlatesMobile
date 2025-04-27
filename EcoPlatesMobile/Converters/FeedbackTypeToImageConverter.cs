using System.Globalization;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile.Converters
{
    public class FeedbackTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string feedbackType)
            {
                var type = PromotionFeedbackTypeExtensions.FromValue(feedbackType);

                switch (type)
                {
                    case PromotionFeedbackType.GREAT_VALUE:
                        return "dollar.png";
                    case PromotionFeedbackType.DELICIOUS_FOOD:
                        return "clap.png";
                    case PromotionFeedbackType.GREAT_SERVICE:
                        return "service.png";
                    default:
                        return "default.png"; // fallback
                }
            }
            return "default.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
