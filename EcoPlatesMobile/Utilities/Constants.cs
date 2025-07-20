using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;

namespace EcoPlatesMobile.Utilities
{
    internal class Constants
    {
        public const string IP = "192.168.219.137";
        //public const string BASE_USER_URL = "http://www.ecoplates.uz:8080/user/";
        public const string BASE_USER_URL = $"http://{IP}:8083";
        //public const string BASE_USER_URL = "http://10.0.2.2:8083";
        //public const string BASE_USER_URL = $"http://{IP}:8083";

        //public const string BASE_COMPANY_URL = "http://www.ecoplates.uz:8080/company/";
        public const string BASE_COMPANY_URL = $"http://{IP}:8081";
        //public const string BASE_COMPANY_URL = "http://10.0.2.2:8081";
        //public const string BASE_COMPANY_URL = $"http://{IP}:8081";

        //public const string BASE_CHAT_URL = "http://10.0.2.2:8085";
        public const string BASE_CHAT_URL = $"http://{IP}:8085"; //home
        //public const string BASE_CHAT_URL = $"http://{IP}:8085"; //company

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

        public static Color COLOR_USER = (Color)Application.Current.Resources["User"];
        public static Color COLOR_COMPANY = (Color)Application.Current.Resources["Company"];
    }
}
