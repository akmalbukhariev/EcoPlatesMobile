using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoreLocation;
using CoreGraphics;
using Foundation;
using MapKit;
using UIKit;
using Microsoft.Maui.ApplicationModel;
using EcoPlatesMobile.Helper;

namespace EcoPlatesMobile.Platforms.iOS
{
    // 1) Annotation that keeps a reference to your CustomPin
    public class CustomPinAnnotation : MKPointAnnotation
    {
        public CustomPin Pin { get; }

        public CustomPinAnnotation(CustomPin pin)
        {
            Pin = pin;
            Coordinate = new CLLocationCoordinate2D(
                pin.Location.Latitude,
                pin.Location.Longitude);

            Title = pin.Label;
        }
    }

    // 2) Helper renderer (NOT a delegate)
    public class PinIconRenderer
    {
        public event Action<CustomPin>? EventPinClick;

        private readonly IList<CustomPin> _pins;

        // pinId → annotation
        private readonly Dictionary<long, CustomPinAnnotation> _annotationByCompanyId = new();

        // pinId → view
        private readonly Dictionary<long, MKAnnotationView> _viewByCompanyId = new();

        // pinId → logo image
        private readonly Dictionary<long, UIImage> _logoByCompanyId = new();

        private readonly TaskCompletionSource<bool> _renderingDone = new();
        public Task RenderingFinished => _renderingDone.Task;

        public PinIconRenderer(IList<CustomPin> pins)
        {
            _pins = pins;
        }

        public void Attach(MKMapView mapView)
        {
            if (!MainThread.IsMainThread)
            {
                MainThread.BeginInvokeOnMainThread(() => Attach(mapView));
                return;
            }

            mapView.DidAddAnnotationViews   -= MapView_DidAddAnnotationViews;
            mapView.DidAddAnnotationViews   += MapView_DidAddAnnotationViews;

            mapView.DidSelectAnnotationView -= MapView_DidSelectAnnotationView;
            mapView.DidSelectAnnotationView += MapView_DidSelectAnnotationView;


            // Clear existing annotations except user location
            var existing = mapView.Annotations;
            if (existing != null && existing.Length > 0)
            {
                var toRemove = existing.Where(a => a is not MKUserLocation).ToArray();
                if (toRemove.Length > 0)
                    mapView.RemoveAnnotations(toRemove);
            }

            // Add our annotations
            _annotationByCompanyId.Clear();
            foreach (var pin in _pins)
            {
                var ann = new CustomPinAnnotation(pin);
                _annotationByCompanyId[pin.CompanyId] = ann;
                mapView.AddAnnotation(ann);
            }

            if (!_renderingDone.Task.IsCompleted)
                _renderingDone.TrySetResult(true);

            // Start loading logos in background
            _ = LoadIconsAsync(mapView);
        }

        private async Task LoadIconsAsync(MKMapView mapView)
        {
            using var http = new HttpClient();

            foreach (var p in _pins)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(p.LogoUrl))
                        continue;

                    var bytes = await http.GetByteArrayAsync(p.LogoUrl).ConfigureAwait(false);
                    if (bytes == null || bytes.Length == 0)
                        continue;

                    UIImage? circleImage = null;

                    // UIKit must be on main thread
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        using var data = NSData.FromArray(bytes);
                        var logo = UIImage.LoadFromData(data);
                        if (logo == null)
                            return;

                        circleImage = MakeCircleImage(logo, 45);
                    });

                    if (circleImage == null)
                        continue;

                    // remember logo
                    _logoByCompanyId[p.CompanyId] = circleImage;

                    // if view already exists, update it
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        if (_viewByCompanyId.TryGetValue(p.CompanyId, out var view))
                        {
                            view.Image = circleImage;
                            view.CanShowCallout = false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadIconsAsync error for {p.CompanyId}: {ex}");
                }
            }
        }

        private UIImage MakeCircleImage(UIImage source, nfloat size)
        {
            var rect = new CGRect(0, 0, size, size);

            UIGraphics.BeginImageContextWithOptions(rect.Size, false, UIScreen.MainScreen.Scale);
            var ctx = UIGraphics.GetCurrentContext();
            if (ctx == null)
            {
                UIGraphics.EndImageContext();
                return source;
            }

            // outer white circle
            ctx.AddEllipseInRect(rect);
            ctx.SetFillColor(UIColor.White.CGColor);
            ctx.FillPath();

            // inner circle for logo
            var innerRect = rect.Inset(4, 4);
            ctx.AddEllipseInRect(innerRect);
            ctx.Clip();

            source.Draw(innerRect);

            var result = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return result ?? source;
        }

        // Called whenever MKMapView creates views for annotations
        private void MapView_DidAddAnnotationViews(object? sender, MKMapViewAnnotationEventArgs e)
        {
            if (sender is not MKMapView mapView)
                return;

            // e.Views is the array of added views
            foreach (var view in e.Views)
            {
                if (view.Annotation is CustomPinAnnotation customAnn)
                {
                    var companyId = customAnn.Pin.CompanyId;

                    // remember the view for this company
                    _viewByCompanyId[companyId] = view;

                    // if logo is already loaded, apply it immediately
                    if (_logoByCompanyId.TryGetValue(companyId, out var img))
                    {
                        view.Image = img;
                        view.CanShowCallout = false;

                        if (view is MKMarkerAnnotationView marker)
                        {
                            marker.GlyphImage = img;                     // put image inside marker
                            marker.GlyphTintColor = UIColor.Clear;       // remove default glyph
                            marker.MarkerTintColor = UIColor.Clear;      // remove red shape
                        }
                    }
                    else
                    {
                        HideUntilLogo(view);
                    }
                }
            }
        }

        private void HideUntilLogo(MKAnnotationView view)
        {
            if (view is MKMarkerAnnotationView marker)
            {
                marker.GlyphImage = null;
                marker.GlyphTintColor = UIColor.Clear;
                marker.MarkerTintColor = UIColor.Clear; // hides the pin
                marker.TitleVisibility = MKFeatureVisibility.Hidden;
                marker.SubtitleVisibility = MKFeatureVisibility.Hidden;
            }

            view.CenterOffset = CGPoint.Empty;
        }

        private void MapView_DidSelectAnnotationView(object? sender, MKAnnotationViewEventArgs e)
        {
            var view = e.View;
            if (view?.Annotation is CustomPinAnnotation ann)
            {
                EventPinClick?.Invoke(ann.Pin);
            }
        }
    }
}