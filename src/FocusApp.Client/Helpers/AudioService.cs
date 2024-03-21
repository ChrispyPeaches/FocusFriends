using CommunityToolkit.Maui.Core.Primitives;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;

namespace FocusApp.Client.Helpers;

public interface IAudioService
{
    void Play(string audioFileName);
    void Stop();
}

public class AudioService : IAudioService
{
    private MediaElement _mediaElement;

    public AudioService()
    {
        _mediaElement = new MediaElement();
    }

    public void Play(string audioFileName)
    {
        _mediaElement.Source = MediaSource.FromFile(audioFileName);
        //_mediaElement.IsVisible = false;
        _mediaElement.ShouldAutoPlay = true;
        Console.WriteLine($"Playing audio file: {audioFileName}");
        Console.WriteLine($"Media Element Source: {_mediaElement.Source}");
    }

    public void Stop()
    {
        _mediaElement.Stop();
    }
}

public static class AudioSettings
{
    public static readonly BindableProperty AmbianceVolumeProperty =
        BindableProperty.Create(
            propertyName: nameof(AmbianceVolume),
            returnType: typeof(double),
            declaringType: typeof(AudioSettings),
            defaultValue: 50.0,
            defaultBindingMode: BindingMode.TwoWay);

    public static double AmbianceVolume
    {
        get { return Preferences.Get("ambiance_volume", 50.0); }
        set { Preferences.Set("ambiance_volume", value); }
    }
}


