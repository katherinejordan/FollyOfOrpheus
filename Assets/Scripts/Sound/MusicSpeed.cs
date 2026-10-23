using UnityEngine;
using UnityEngine.Audio;

public class MusicSpeed : MonoBehaviour
{
    private const string TempoParam = "MusicPitch";
    private const string VolumeParam = "MusicSpeedVolume";

    [SerializeField] private PlayerController player;
    [SerializeField] private AudioMixer mixer;

    [Header("Curves")]
    [SerializeField] private AnimationCurve tempoBySpeed = AnimationCurve.Linear(7f, 1f, 150f, 1.3f);
    [SerializeField] private AnimationCurve volumeBySpeed = AnimationCurve.Linear(7f, 0.4f, 60f, 1f);

    [Header("Smoothing")]
    [SerializeField] private float catchUpRate = 40f;

    private float smoothedSpeed;

    private void Update()
    {
        if (player == null) return;
        smoothedSpeed = Mathf.MoveTowards(smoothedSpeed, player.Speed, catchUpRate * Time.deltaTime);

        Apply(tempoBySpeed.Evaluate(smoothedSpeed), volumeBySpeed.Evaluate(smoothedSpeed));
    }

    private void OnDisable()
    {
        Apply(1f, 1f);
    }

    private void Apply(float tempo, float volume)
    {
        if (mixer == null) return;

        mixer.SetFloat(TempoParam, tempo);
        float decibels = Mathf.Log10(Mathf.Max(Mathf.Clamp01(volume), 0.0001f)) * 20f;
        mixer.SetFloat(VolumeParam, decibels);
    }
}