
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

            // Bind to CollectionViews
            DateList.ItemsSource = Dates;
            HourList.ItemsSource = Hours;
            MinuteList.ItemsSource = Minutes;

            // Set default selections
            DateList.SelectedItem = Dates.FirstOrDefault();
            HourList.SelectedItem = Hours.FirstOrDefault();
            MinuteList.SelectedItem = Minutes.FirstOrDefault();
        }

        /// <summary>
        /// Show the date picker with animation
        /// </summary>
        public async void Show()
        {
            IsVisible = true;
            PickerFrame.TranslationY = 300;
            await PickerFrame.TranslateTo(0, 0, 300, Easing.SinOut);
        }

        /// <summary>
        /// Hide the date picker with animation
        /// </summary>
        public async void Hide()
        {
            await PickerFrame.TranslateTo(0, 300, 300, Easing.SinIn);
            IsVisible = false;
        }

        /// <summary>
        /// Tapped outside = hide the picker
        /// </summary>
        private void OnOverlayTapped(object sender, EventArgs e)
        {
            Hide();
        }

        /// <summary>
        /// Confirm selected values
        /// </summary>
        private void OnConfirmClicked(object sender, EventArgs e)
        {
            if (DateList.SelectedItem == null || HourList.SelectedItem == null || MinuteList.SelectedItem == null)
                return;

            string selected = $"{DateList.SelectedItem} {HourList.SelectedItem}:{MinuteList.SelectedItem}";
            DateSelected?.Invoke(this, selected);
            Hide();
        }
    }