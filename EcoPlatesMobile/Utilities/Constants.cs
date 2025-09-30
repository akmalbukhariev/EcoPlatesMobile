﻿using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;

namespace EcoPlatesMobile.Utilities
{
    internal class Constants
    {
        //public const string IP = "192.168.0.54";
        //public const string IP = "192.168.219.164";
        //public const string IP = "10.0.2.2";
        public const string SERVER_DOMAIN = "www.saletop.uz";
        //public const string SERVER_DOMAIN = "95.182.118.233";

        public const string BASE_USER_URL = $"http://{SERVER_DOMAIN}/user/";
        //public const string BASE_USER_URL = $"http://{IP}:8083";

        public const string BASE_COMPANY_URL = $"http://{SERVER_DOMAIN}/company/";
        //public const string BASE_COMPANY_URL = $"http://{IP}:8081";

        public const string BASE_CHAT_URL = $"http://{SERVER_DOMAIN}/chatting/";
        //public const string BASE_CHAT_URL = $"http://{IP}:8085";

        public const string BASE_PHONE_VERIFY_URL = $"http://{SERVER_DOMAIN}/message/";
        //public const string BASE_PHONE_VERIFY_URL = $"http://{IP}:8087";

        public static readonly string Version = AppInfo.Current.VersionString;     // e.g., "1.0"
        public static readonly string Build = AppInfo.Current.BuildString;         // e.g., "1.0.0"

        public static readonly string OsName = DeviceInfo.Current.Platform.ToString();      // Android, iOS, macOS, Windows
        public static readonly string OsVersion = DeviceInfo.Current.VersionString;         // OS version string

        public const int MaxRadius = 10;

        public const string UZ = "uz";
        public const string EN = "en";
        public const string RU = "ru";

        public const string ROLE_USER = "ROLE_USER";
        public const string ROLE_COMPANY = "ROLE_COMPANY";

        public const string NOTIFICATION_TITLE = "notification_title";
        public const string NOTIFICATION_BODY = "notification_body";
        public const string SEARCH_NOTIFICATION_FOR_USER = "SearchNotificationForUser";
        public const string SEARCH_NOTIFICATION_FOR_COMPANY = "SearchNotificationForCompany";

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
