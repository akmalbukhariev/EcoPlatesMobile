using Android.Gms.Maps;

public class ZoomControlDisabler : Java.Lang.Object, IOnMapReadyCallback
{
    public void OnMapReady(GoogleMap googleMap)
    {
        googleMap.UiSettings.ZoomControlsEnabled = false;
    }
}
