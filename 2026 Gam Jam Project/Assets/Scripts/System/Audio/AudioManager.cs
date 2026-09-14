using System.Collections.Generic;
using Unity.VectorGraphics.Editor;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Default Mixer Groups")]
    [SerializeField] private AudioMixerGroup defaultMusicGroup;
    [SerializeField] private AudioMixerGroup defaultSfxGroup;

    [Header("Audio Libraries")]
    [SerializeField] private List<BGM> bgmLibrary;
    [SerializeField] private List<SFX> sfxLibrary;

    public AudioSource BGMSource => bgmSource;
    public AudioSource SFXSource => sfxSource;
    public AudioMixerGroup DefaultMusicGroup => defaultMusicGroup;
    public AudioMixerGroup DefaultSfxGroup => defaultSfxGroup;
    private Dictionary<BGMType, IPlayable> bgmMap;
    private Dictionary<SFXType, IPlayable> sfxMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build BGM lookup
        bgmMap = new Dictionary<BGMType, IPlayable>();
        foreach (var bgm in bgmLibrary)
            bgmMap[bgm.Type] = bgm;

        // Build SFX lookup
        sfxMap = new Dictionary<SFXType, IPlayable>();
        foreach (var sfx in sfxLibrary)
            sfxMap[sfx.Type] = sfx;
    }

    private void OnEnable()
    {
        AudioEvents.OnBGMRequested += HandleBGMRequest;
        AudioEvents.OnSFXRequested += HandleSFXRequest;
        AudioEvents.OnAudioRequested += Play;
    }

    private void OnDisable()
    {
        AudioEvents.OnBGMRequested -= HandleBGMRequest;
        AudioEvents.OnSFXRequested -= HandleSFXRequest;
        AudioEvents.OnAudioRequested -= Play;
    }

    /// <summary>
    /// Plays any IPlayable object.
    /// Looping playables use the BGM source; non-looping use the SFX source.
    /// </summary>
    public void Play(IPlayable playable)
    {
        if (playable == null || playable.Clip == null)
            return;

        AudioSource source = playable.Source;

        source.clip = playable.Clip;
        source.volume = playable.Volume;
        source.loop = playable.Loop;

        source.outputAudioMixerGroup = playable.MixerGroup;

        if (playable.Loop)
            source.Play();
        else
            source.PlayOneShot(playable.Clip, playable.Volume);
    }

    /// <summary>
    /// Pauses playback for looping IPlayable types (BGM).
    /// </summary>
    public void Pause(IPlayable playable)
    {
        if (bgmSource.isPlaying)
            bgmSource.Pause();
    }

    /// <summary>
    /// Stops the current looping and begins playback of the next one.
    /// </summary>
    public void Transition(IPlayable from, IPlayable to)
    {
        bgmSource.Stop();
        Play(to);
    }

    /// <summary>
    /// Handles BGM requests fired by gameplay via enum identity.
    /// </summary>
    private void HandleBGMRequest(BGMType type)
    {
        if (bgmMap.TryGetValue(type, out IPlayable bgm))
            Play(bgm);
    }

    /// <summary>
    /// Handles SFX requests fired by gameplay via enum identity.
    /// </summary>
    private void HandleSFXRequest(SFXType type)
    {
        if (sfxMap.TryGetValue(type, out IPlayable sfx))
            Play(sfx);
    }
}
