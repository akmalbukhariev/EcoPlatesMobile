
using System.Collections.ObjectModel;

namespace EcoPlatesMobile.Views.Components;

public partial class CustomDatePickerView : ContentView
{ 
    public event EventHandler<string> DateSelected;

        public ObservableCollection<string> Dates { get; set; }
        public ObservableCollection<string> Hours { get; set; }
        public ObservableCollection<string> Minutes { get; set; }

        public CustomDatePickerView()
        {
            InitializeComponent();

            // Populate date list (Next 7 days)
            Dates = new ObservableCollection<string>();
            for (int i = 0; i < 7; i++)
            {
                Dates.Add(DateTime.Now.AddDays(i).ToString("MMM d"));
            }

            // Populate hour list (0-23)
            Hours = new ObservableCollection<string>();
            for (int i = 0; i < 24; i++)
            {
                Hours.Add(i.ToString("D2"));
            }

            // Populate minute list (0-59)
            Minutes = new ObservableCollection<string>();
            for (int i = 0; i < 60; i++)
            {
                Minutes.Add(i.ToString("D2"));
            }

            // Bind lists to UI
            DateList.ItemsSource = Dates;
            HourList.ItemsSource = Hours;
            MinuteList.ItemsSource = Minutes;
        }

        /// <summary>
        /// Show the Date Picker with a slide-up animation
        /// </summary>
        public async void Show()
        {
            IsVisible = true;
            PickerFrame.TranslationY = 300; // Start position (off-screen)
            await PickerFrame.TranslateTo(0, 0, 300, Easing.SinOut);
        }

        /// <summary>
        /// Hide the Date Picker with a slide-down animation
        /// </summary>
        public async void Hide()
        {
            await PickerFrame.TranslateTo(0, 300, 300, Easing.SinIn);
            IsVisible = false;
        }

        /// <summary>
        /// Closes the picker when the background overlay is tapped
        /// </summary>
        private void OnOverlayTapped(object sender, EventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// Handles the confirmation button click, returning the selected date/time
        /// </summary>
        private void OnConfirmClicked(object sender, EventArgs e)
        {
            if (DateList.SelectedItem == null || HourList.SelectedItem == null || MinuteList.SelectedItem == null)
                return;

            string selectedDate = $"{DateList.SelectedItem} {HourList.SelectedItem}:{MinuteList.SelectedItem}";
            DateSelected?.Invoke(this, selectedDate); // Fire event
            Hide();
        }
}