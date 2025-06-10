using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Utilities
{
    public abstract class SafeModelBase
    {
        protected string SafeString(string? value) => value ?? string.Empty;

        protected int SafeInt(object? value)
        {
            if (value == null) return 0;
            return int.TryParse(value.ToString(), out var result) ? result : 0;
        }

        protected long SafeLong(object? value)
        {
            if (value == null) return 0;
            return long.TryParse(value.ToString(), out var result) ? result : 0;
        }

        protected double SafeDouble(object? value)
        {
            if (value == null) return 0;
            return double.TryParse(value.ToString(), out var result) ? result : 0;
        }

        protected DateTime SafeDate(object? value)
        {
            if (value == null) return DateTime.UtcNow;

            if (long.TryParse(value.ToString(), out var unix))
                return DateTimeOffset.FromUnixTimeMilliseconds(unix).UtcDateTime;

            if (DateTime.TryParse(value.ToString(), out var dt))
                return dt;

            return DateTime.UtcNow;
        }
    }

}
