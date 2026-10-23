using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// One "sound" in the game: a set of clip variations plus how to play them.
/// Create via: Right-click in Project window > Create > Audio > Sound Data
/// </summary>
[CreateAssetMenu(fileName = "NewSound", menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    [Header("Clips")]
    [Tooltip("If there is more than one, a random one plays each time (never the same one twice in a row).")]
    public AudioClip[] clips;

    [Header("Mixing")]
    [Tooltip("Which mixer group this sound plays through (SFX, UI, Ambience...).")]
    public AudioMixerGroup mixerGroup;
    [Range(0f, 1f)] public float volume = 1f;

    [Header("Variation")]
    [Tooltip("A random pitch between X (min) and Y (max) is picked each play. 1 = normal pitch.")]
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    [Header("3D Settings")]
    [Tooltip("0 = sounds the same everywhere (2D). 1 = fully positioned in the 3D world.")]
    [Range(0f, 1f)] public float spatialBlend = 1f;
    [Tooltip("Within this distance the sound is at full volume.")]
    public float minDistance = 1f;
    [Tooltip("Beyond this distance the sound stops getting quieter.")]
    public float maxDistance = 30f;

    // NonSerialized so this runtime-only value never gets saved into the asset.
    [System.NonSerialized] private int lastIndex = -1;

    public bool HasClips => clips != null && clips.Length > 0;

    /// <summary>Picks a random clip, avoiding an immediate repeat.</summary>
    public AudioClip GetClip()
    {
        if (!HasClips) return null;
        if (clips.Length == 1) return clips[0];

        int index;
        do
        {
            index = Random.Range(0, clips.Length);
        } while (index == lastIndex);

        lastIndex = index;
        return clips[index];
    }

    public float GetPitch() => Random.Range(pitchRange.x, pitchRange.y);
}