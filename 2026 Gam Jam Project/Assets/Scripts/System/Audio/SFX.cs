using Unity.VectorGraphics.Editor;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/SFX")]
public class SFX : ScriptableObject, IPlayable
{
    [SerializeField] private SFXType _sfxType;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private float _volume = 1f;
    [SerializeField] private bool _loop = true;
    [SerializeField] private AudioSource _audioSource;

    public string Name => _sfxType.ToString();

    public SFXType Type => _sfxType;
    public AudioClip Clip => _clip;
    public float Volume => _volume;
    public bool Loop => _loop;
    public AudioSource Source => AudioManager.Instance.SFXSource;
    public AudioMixerGroup MixerGroup => AudioManager.Instance.DefaultSfxGroup;

    public string GetName() => _sfxType.ToString();

    /// <summary>
    /// Requests playback of this SFX through the AudioManager.
    /// </summary>
    public void Play()
    {
        AudioManager.Instance.Play(this);
    }

    /// <summary>
    /// SFX cannot be paused; 
    /// </summary>
    public void Pause() { }

    /// <summary>
    /// SFX cannot transition;
    /// </summary>
    public void TransitionTo(IPlayable playable) { }
}

public enum SFXType
{
    PlayerWalk,
    PlayerHide,
    PlayerGrab
}
