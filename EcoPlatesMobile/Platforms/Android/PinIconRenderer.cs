using Android.Gms.Common.Apis;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using EcoPlatesMobile.Helper;
using EcoPlatesMobile.Views.User.Pages;
using Microsoft.Maui.Controls.Maps;
using System.Net.Http;

namespace EcoPlatesMobile.Platforms.Android
{
    public class CustomPinWrapper : Java.Lang.Object
    {
        public CustomPin Pin { get; }

        public CustomPinWrapper(CustomPin pin)
        {
            Pin = pin;
        }
    }

    public class PinIconRenderer : Java.Lang.Object, IOnMapReadyCallback
    {
        public event Action<CustomPin> EventPinClick;
        readonly IList<CustomPin> _pins;

        public PinIconRenderer(IList<CustomPin> pins)
        {
            _pins = pins;
        }

        public async void OnMapReady(GoogleMap googleMap)
        {
            googleMap.Clear();

            foreach (var pin in _pins.ToList())
            {
                if (pin is CustomPin customPin)
                {
                    //if (customPin.IsPin) continue;

                    var location = new LatLng(customPin.Location.Latitude, customPin.Location.Longitude);
                    BitmapDescriptor icon = BitmapDescriptorFactory.DefaultMarker();

                    try
                    {
                        using var http = new HttpClient();
                        var bytes = await http.GetByteArrayAsync(customPin.LogoUrl);
                        if (bytes?.Length > 0)
                        {
                            var logo = await global::Android.Graphics.BitmapFactory.DecodeByteArrayAsync(bytes, 0, bytes.Length);

                            int size = 100;
                            var outputBitmap = global::Android.Graphics.Bitmap.CreateBitmap(size, size, global::Android.Graphics.Bitmap.Config.Argb8888);
                            var canvas = new global::Android.Graphics.Canvas(outputBitmap);

                            // Draw white circular background
                            var paintBg = new global::Android.Graphics.Paint
                            {
                                AntiAlias = true,
                                Color = global::Android.Graphics.Color.White
                            };
                            canvas.DrawCircle(size / 2, size / 2, size / 2, paintBg);

                            // Draw logo clipped to a circular shader
                            var shader = new global::Android.Graphics.BitmapShader(
                                global::Android.Graphics.Bitmap.CreateScaledBitmap(logo, size, size, false),
                                global::Android.Graphics.Shader.TileMode.Clamp,
                                global::Android.Graphics.Shader.TileMode.Clamp
                            );

                            var paintLogo = new global::Android.Graphics.Paint
                            {
                                AntiAlias = true
                            };
                            paintLogo.SetShader(shader);

                            canvas.DrawCircle(size / 2, size / 2, size / 2 - 4, paintLogo);

                            // Convert to map icon
                            icon = BitmapDescriptorFactory.FromBitmap(outputBitmap);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[PinIconRenderer] Failed to load logo: {ex.Message}");
                    }

                    var markerOptions = new MarkerOptions()
                            .SetPosition(location)
                            .SetIcon(icon);

                    var marker = googleMap.AddMarker(markerOptions);
                    marker.Tag = new CustomPinWrapper(customPin);
                } 
            }
            
            googleMap.MarkerClick += (s, e) =>
            {
                if (e.Marker?.Tag is CustomPinWrapper wrapper)
                {
                    EventPinClick?.Invoke(wrapper.Pin);
                }

                e.Handled = true;
            };
        }
    }
}