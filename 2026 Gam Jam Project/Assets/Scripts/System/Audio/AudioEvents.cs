using System;

public static class AudioEvents
{
    // Gameplay requests a BGM change using enum identity
    public static Action<BGMType> OnBGMRequested;

    // Gameplay requests an SFX using enum identity
    public static Action<SFXType> OnSFXRequested;

    // Gameplay or systems can request audio directly via IPlayable
    public static Action<IPlayable> OnAudioRequested;
}
