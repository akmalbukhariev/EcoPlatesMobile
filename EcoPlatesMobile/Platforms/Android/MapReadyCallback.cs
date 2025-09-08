using Android.Gms.Maps;

namespace EcoPlatesMobile.Platforms.Android
{
    public class MapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
    {
        private readonly Action<GoogleMap> _onReady;

        public MapReadyCallback(Action<GoogleMap> onReady)
        {
            _onReady = onReady;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _onReady?.Invoke(googleMap);
        }
    }
}