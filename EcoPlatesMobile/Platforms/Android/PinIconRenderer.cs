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
        private TaskCompletionSource<bool> _renderingDone = new();
        public Task RenderingFinished => _renderingDone.Task;

        public PinIconRenderer(IList<CustomPin> pins)
        {
            _pins = pins;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            try
            {
                googleMap.Clear();

                // 1) Add markers quickly with default icons (no awaits here)
                var markerByPin = new Dictionary<long, Marker>();
                foreach (var p in _pins)
                {
                    var loc = new LatLng(p.Location.Latitude, p.Location.Longitude);
                    var marker = googleMap.AddMarker(new MarkerOptions()
                        .SetPosition(loc)
                        .SetIcon(BitmapDescriptorFactory.DefaultMarker()));
                    marker.Tag = new CustomPinWrapper(p);
                    markerByPin[p.CompanyId] = marker;
                }

                // 2) Hook clicks (still light)
                googleMap.MarkerClick += (s, e) =>
                {
                    if (e.Marker?.Tag is CustomPinWrapper w) EventPinClick?.Invoke(w.Pin);
                    e.Handled = true;
                };

                // 3) Tell caller we're done so UI can continue
                _renderingDone.TrySetResult(true);

                // 4) Load and apply logos in background (no blocking UI)
                _ = Task.Run(async () =>
                {
                    using var http = new HttpClient();
                    foreach (var p in _pins)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(p.LogoUrl)) continue;

                            var bytes = await http.GetByteArrayAsync(p.LogoUrl).ConfigureAwait(false);
                            if (bytes?.Length == 0) continue;

                            var logo = await global::Android.Graphics.BitmapFactory
                                .DecodeByteArrayAsync(bytes, 0, bytes.Length)
                                .ConfigureAwait(false);
                            if (logo is null) continue;

                            // circle compose (same as your code)
                            int size = 100;
                            var output = global::Android.Graphics.Bitmap.CreateBitmap(size, size, global::Android.Graphics.Bitmap.Config.Argb8888);
                            var canvas = new global::Android.Graphics.Canvas(output);

                            var paintBg = new global::Android.Graphics.Paint { AntiAlias = true, Color = global::Android.Graphics.Color.White };
                            canvas.DrawCircle(size / 2, size / 2, size / 2, paintBg);

                            var shader = new global::Android.Graphics.BitmapShader(
                                global::Android.Graphics.Bitmap.CreateScaledBitmap(logo, size, size, false),
                                global::Android.Graphics.Shader.TileMode.Clamp,
                                global::Android.Graphics.Shader.TileMode.Clamp);
                            var paintLogo = new global::Android.Graphics.Paint { AntiAlias = true };
                            paintLogo.SetShader(shader);
                            canvas.DrawCircle(size / 2, size / 2, size / 2 - 4, paintLogo);

                            var icon = BitmapDescriptorFactory.FromBitmap(output);

                            // apply on UI thread
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                if (markerByPin.TryGetValue(p.CompanyId, out var m))
                                    m.SetIcon(icon);
                            });
                        }
                        catch { /* ignore individual icon failures */ }
                    }
                });
            }
            catch (Exception ex)
            {
                _renderingDone.TrySetException(ex);
            }
        }

        /*
        public async void OnMapReady(GoogleMap googleMap)
        {
            try
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

                                var paintBg = new global::Android.Graphics.Paint
                                {
                                    AntiAlias = true,
                                    Color = global::Android.Graphics.Color.White
                                };
                                canvas.DrawCircle(size / 2, size / 2, size / 2, paintBg);

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

                _renderingDone.TrySetResult(true);
            }
            catch (Exception ex)
            {
                _renderingDone.TrySetException(ex);
            }
        }
        */
    }
}