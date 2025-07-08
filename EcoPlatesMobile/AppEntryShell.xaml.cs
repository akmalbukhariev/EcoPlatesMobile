using EcoPlatesMobile.Services;
using EcoPlatesMobile.Views;

namespace EcoPlatesMobile
{
    public partial class AppEntryShell : Shell
    {
        public AppEntryShell()
        {
            InitializeComponent();
            
            Init();
        }

        private void Init()
        {
            Items.Clear();

            bool isLanguageSet = AppService.Get<AppStoreService>().Get(AppKeys.IsLanguageSet, false);
            if (isLanguageSet)
            {
                var loginItem = new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(LoginPage))
                };

                Items.Add(loginItem);
            }
            else
            {
                var languageItem = new ShellContent
                {
                    ContentTemplate = new DataTemplate(typeof(LanguagePage))
                };

                Items.Add(languageItem);
            }
        }
    }
}
