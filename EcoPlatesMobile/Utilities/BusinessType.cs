using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Utilities
{
    public enum BusinessType
    {
        RESTAURANT,
        BAKERY,
        CAFE,
        FAST_FOOD,
        SUPERMARKET,
        OTHER
    }

    public static class BusinessTypeExtensions
    {
        public static string GetValue(this BusinessType type)
        {
            return type.ToString();
        }

        public static BusinessType FromValue(string value)
        {
            if (Enum.TryParse<BusinessType>(value, true, out var result))
            {
                return result;
            }

            throw new ArgumentException($"Unknown BusinessType value: {value}");
        }
    }
}
