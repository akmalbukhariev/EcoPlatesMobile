using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;

namespace EcoPlatesMobile.Utilities
{
    internal class Constants
    {
        public const string BASE_USER_URL = "http://www.ecoplates.uz:8080/user/";
        //public const string BASE_USER_URL = "http://192.168.219.132:8083";
        //public const string BASE_USER_URL = "http://10.0.2.2:8083";
        
        public const string BASE_COMPANY_URL = "http://www.ecoplates.uz:8080/company/";
        //public const string BASE_COMPANY_URL = "http://192.168.219.132:8081";
        //public const string BASE_COMPANY_URL = "http://10.0.2.2:8081";

        public static readonly string Version = AppInfo.Current.VersionString;     // e.g., "1.0"
        public static readonly string Build = AppInfo.Current.BuildString;         // e.g., "1.0.0"

        public static readonly string OsName = DeviceInfo.Current.Platform.ToString();      // Android, iOS, macOS, Windows
        public static readonly string OsVersion = DeviceInfo.Current.VersionString;         // OS version string

        public const string UZ = "uz";
        public const string EN = "en";
        public const string RU = "ru";

        public const string LAN_UZBEK = "O'zbekcha";
        public const string LAN_ENGLISH = "English";
        public const string LAN_RUSSIAN = "Русский";

        public const string LAN_ICON_UZBEK = "flag_uz.png";
        public const string LAN_ICON_ENGLISH = "flag_en.png";
        public const string LAN_ICON_RUSSIAN = "flag_ru.png";

        public const string PHONE_PATTERN = @"^(\+998|998)?(90|91|93|94|95|97|98|99|33|88|20)\d{7}$";
    }
}
