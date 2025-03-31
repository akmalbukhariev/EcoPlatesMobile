using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoPlatesMobile.Views.Components
{
    public class LoadingView : ContentView
    {
        private readonly ActivityIndicator _activityIndicator;

        public static readonly BindableProperty IsLoadingProperty =
            BindableProperty.Create(
                nameof(IsLoading),
                typeof(bool),
                typeof(LoadingView),
                false,
                propertyChanged: OnIsLoadingChanged);

        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public LoadingView()
        {
            BackgroundColor = new Color(0f, 0f, 0f, 0.5f);
            IsVisible = false;

            _activityIndicator = new ActivityIndicator
            {
                IsRunning = false,
                IsVisible = false,
                Color = Colors.Green,
                Scale = 1.5
            };

            var layout = new Grid
            {
                RowDefinitions = { new RowDefinition { Height = GridLength.Star } },
                ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Star } }
            };

            layout.Children.Add(_activityIndicator);
            Grid.SetRow(_activityIndicator, 0);
            Grid.SetColumn(_activityIndicator, 0);
            layout.HorizontalOptions = LayoutOptions.Center;
            layout.VerticalOptions = LayoutOptions.Center;

            Content = layout;
        }

        private static async void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (LoadingView)bindable;
            bool isLoading = (bool)newValue;

            if (isLoading)
            {
                view.IsVisible = true;
                view._activityIndicator.IsVisible = true;
                view._activityIndicator.IsRunning = true;

                view.Opacity = 0;
                await view.FadeTo(1, 200);
 
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000);
                    if (view.IsLoading)
                    {
                        MainThread.BeginInvokeOnMainThread(() => view.IsLoading = false);
                    }
                });
            }
            else
            {
                await view.FadeTo(0, 200);
                view._activityIndicator.IsRunning = false;
                view._activityIndicator.IsVisible = false;
                view.IsVisible = false;
            }
        }

        /*private static async void OnIsLoadingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (LoadingView)bindable;
            bool isLoading = (bool)newValue;

            if (isLoading)
            {
                view.IsVisible = true;
                view._activityIndicator.IsVisible = true;
                view._activityIndicator.IsRunning = true;

                view.Opacity = 0;
                await view.FadeTo(1, 200);
            }
            else
            {
                await view.FadeTo(0, 200);
                view._activityIndicator.IsRunning = false;
                view._activityIndicator.IsVisible = false;
                view.IsVisible = false;
            }
        }*/
    }
}
