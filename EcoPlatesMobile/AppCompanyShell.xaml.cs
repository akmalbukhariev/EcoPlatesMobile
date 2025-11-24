
using EcoPlatesMobile.Services;
using EcoPlatesMobile.Utilities;

namespace EcoPlatesMobile
{
    public partial class AppCompanyShell : Shell
    {
        public AppCompanyShell()
        {
            InitializeComponent();

            this.Navigated += OnShellNavigated;
        }

        private void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
        {
            // Ensure we are on UI thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DependencyService.Get<IStatusBarService>()?
                        .SetStatusBarColor(Constants.COLOR_COMPANY.ToArgbHex(), false);
            });
        }
    }
}