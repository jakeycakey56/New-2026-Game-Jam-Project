
using UnityEngine;

using UnityEngine.Audio;

///<summary>
/// Contains the interface for playable objects in the audio system.
///</summary>
public interface IPlayable
{
    AudioClip Clip { get; }
    float Volume { get; }
    bool Loop { get; }
    AudioSource Source { get; }
    AudioMixerGroup MixerGroup { get; }

    public void Play();

    public void Pause();

    public void TransitionTo(IPlayable playable);

    public string GetName();
}
