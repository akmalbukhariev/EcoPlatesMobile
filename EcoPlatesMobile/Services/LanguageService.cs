 
using EcoPlatesMobile.Resources.Languages;
using EcoPlatesMobile.Utilities;
using HarfBuzzSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Services
{
    public class LanguageService
    {
        private const string LanguageKey = "AppLanguage";
        private const string DefaultLanguage = Constants.EN;

        private readonly AppStoreService _appStore;

        public LanguageService(AppStoreService appStore)
        {
            _appStore = appStore;
        }

        public void Init()
        {
            string lang = _appStore.Get<string>(LanguageKey, DefaultLanguage);
            SetCulture(lang);
        }

        public void SetCulture(string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            AppResource.Culture = culture;

            _appStore.Set(LanguageKey, cultureCode);
        }

        public string GetCurrentLanguage()
        {
            return _appStore.Get<string>(LanguageKey, DefaultLanguage);
        }

        public string GetString(string key)
        {
            return AppResource.ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
        }
    }
}
