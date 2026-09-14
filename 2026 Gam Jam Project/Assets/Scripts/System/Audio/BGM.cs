using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio/BGM")]
public class BGM : ScriptableObject, IPlayable
{
    [SerializeField] private BGMType _bgmType;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private float _volume = 1f;
    [SerializeField] private bool _loop = true;
    [SerializeField] private AudioSource _audioSource;

    public string Name => _bgmType.ToString();

    public BGMType Type => _bgmType;
    public AudioClip Clip => _clip;
    public float Volume => _volume;
    public bool Loop => _loop;
    public AudioSource Source => AudioManager.Instance.BGMSource;
    public AudioMixerGroup MixerGroup => AudioManager.Instance.DefaultMusicGroup;

    public string GetName() => _bgmType.ToString();

    /// <summary>
    /// Requests playback of this BGM through the AudioManager.
    /// </summary>
    public void Play()
    {
        AudioManager.Instance.Play(this);
    }

    /// <summary>
    /// Requests pause of this BGM through the AudioManager.
    /// </summary>
    public void Pause()
    {
        AudioManager.Instance.Pause(this);
    }

    /// <summary>
    /// Requests a transition from this BGM to another IPlayable.
    /// </summary>
    public void TransitionTo(IPlayable playable)
    {
        AudioManager.Instance.Transition(this, playable);
    }
}

public enum BGMType
{
    Main_Menu,
    Gameplay,
    Victory,
    Defeat
}
