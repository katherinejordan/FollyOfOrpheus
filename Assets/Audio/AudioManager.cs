using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Central place for all audio. Persists across scenes.
///   SFX:    AudioManager.Instance?.Play(sound);               // 2D (UI)
///           AudioManager.Instance?.Play(sound, worldPos);     // 3D (in the world)
///   Music:  AudioManager.Instance?.PlayMusic(clip, 1.5f);     // crossfades
///   Volume: AudioManager.Instance?.SetVolume("MusicVolume", 0.5f);
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // These must match the exposed parameter names in your Audio Mixer.
    public const string MasterParam = "MasterVolume";
    public const string MusicParam = "MusicVolume";
    public const string SFXParam = "SFXVolume";
    public const string AmbienceParam = "AmbienceVolume";
    public const string UIParam = "UIVolume";
    private static readonly string[] AllParams = { MasterParam, MusicParam, SFXParam, AmbienceParam, UIParam };

    [Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip clip;
    }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [Tooltip("The Music group inside your mixer. Music sources play through this.")]
    [SerializeField] private AudioMixerGroup musicGroup;

    [Header("SFX Pool")]
    [Tooltip("How many sound effects can play at the same time.")]
    [SerializeField] private int poolSize = 16;

    [Header("Music")]
    [Tooltip("Music that starts automatically when a scene loads. Scenes not listed keep the current music.")]
    [SerializeField] private List<SceneMusic> sceneMusic = new();
    [SerializeField] private float sceneFadeTime = 1.5f;

    private readonly List<AudioSource> pool = new();
    private int nextStealIndex;

    private AudioSource musicA;
    private AudioSource musicB;
    private AudioSource currentMusic;
    private AudioSource fadingOutSource;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildPool();
        BuildMusicSources();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (Instance != this) return;
        foreach (string param in AllParams)
            ApplyVolume(param, GetVolume(param));

        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void BuildPool()
    {
        int count = Mathf.Max(1, poolSize);
        for (int i = 0; i < count; i++)
        {
            GameObject go = new GameObject($"PooledSource_{i}");
            go.transform.SetParent(transform);

            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            pool.Add(source);
        }
    }

    private void BuildMusicSources()
    {
        musicA = CreateMusicSource("Music_A");
        musicB = CreateMusicSource("Music_B");
    }

    private AudioSource CreateMusicSource(string sourceName)
    {
        GameObject go = new GameObject(sourceName);
        go.transform.SetParent(transform);

        AudioSource source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f; // music is always 2D
        source.volume = 0f;
        source.outputAudioMixerGroup = musicGroup;
        return source;
    }

    /// <summary>plays a sound in 2d(same volume everywhere).</summary>
    public AudioSource Play(SoundData sound)
    {
        return PlayInternal(sound, Vector3.zero, false);
    }

    /// <summary>plays a sound at a position in the world, using the sound's 3d settings.</summary>
    public AudioSource Play(SoundData sound, Vector3 position)
    {
        return PlayInternal(sound, position, true);
    }

    private AudioSource PlayInternal(SoundData sound, Vector3 position, bool positional)
    {
        if (sound == null || !sound.HasClips) return null;
        AudioClip clip = sound.GetClip();
        if (clip == null) return null;

        AudioSource source = GetFreeSource();
        source.transform.position = position;
        source.clip = clip;
        source.volume = sound.volume;
        source.pitch = sound.GetPitch();
        source.outputAudioMixerGroup = sound.mixerGroup;
        source.loop = sound.loop;

        source.spatialBlend = positional ? sound.spatialBlend : 0f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.minDistance = sound.minDistance;
        source.maxDistance = sound.maxDistance;

        source.Play();
        return source;
    }

    private AudioSource GetFreeSource()
    {
        foreach (AudioSource s in pool)
        {
            if (!s.isPlaying) return s;
        }
        AudioSource stolen = pool[nextStealIndex];
        nextStealIndex = (nextStealIndex + 1) % pool.Count;
        stolen.Stop();
        return stolen;
    }

    /// <summary>Crossfades to a new music clip. Pass null to fade the music out.</summary>
    public void PlayMusic(AudioClip clip, float fadeTime = 1.5f)
    {
        if (clip != null && currentMusic != null && currentMusic.clip == clip) return;
        if (clip == null && currentMusic == null) return;

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            if (fadingOutSource != null)
            {
                fadingOutSource.Stop();
                fadingOutSource.volume = 0f;
                fadingOutSource = null;
            }
        }

        fadeRoutine = StartCoroutine(CrossfadeRoutine(clip, fadeTime));
    }

    public void StopMusic(float fadeTime = 1.5f) => PlayMusic(null, fadeTime);

    private IEnumerator CrossfadeRoutine(AudioClip clip, float fadeTime)
    {
        AudioSource outgoing = currentMusic;
        AudioSource incoming = null;

        if (clip != null)
        {
            incoming = (outgoing == musicA) ? musicB : musicA;
            incoming.clip = clip;
            incoming.loop = true;
            incoming.volume = 0f;
            incoming.Play();
        }

        currentMusic = incoming;
        fadingOutSource = outgoing;

        float outgoingStart = outgoing != null ? outgoing.volume : 0f;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeTime);

            if (incoming != null) incoming.volume = k;
            if (outgoing != null) outgoing.volume = outgoingStart * (1f - k);
            yield return null;
        }

        if (incoming != null) incoming.volume = 1f;
        if (outgoing != null)
        {
            outgoing.Stop();
            outgoing.volume = 0f;
        }

        fadingOutSource = null;
        fadeRoutine = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (SceneMusic entry in sceneMusic)
        {
            if (entry.sceneName == scene.name)
            {
                PlayMusic(entry.clip, sceneFadeTime);
                return;
            }
        }
    }

    /// <summary>Sets a mixer group's volume. value is 0 (silent) to 1 (full). Saved automatically.</summary>
    public void SetVolume(string parameter, float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(parameter, value);
        ApplyVolume(parameter, value);
    }

    /// <summary>Returns the saved volume (0 to 1). Defaults to 1.</summary>
    public float GetVolume(string parameter) => PlayerPrefs.GetFloat(parameter, 1f);

    private void ApplyVolume(string parameter, float value)
    {
        if (mixer == null) return;
        float decibels = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f;

        if (!mixer.SetFloat(parameter, decibels))
            Debug.LogWarning($"AudioManager: mixer has no exposed parameter named '{parameter}'.");
    }
}