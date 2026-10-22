using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Put this on a UI Slider in your settings menu. No other wiring needed.
/// Set "Parameter" to MasterVolume, MusicVolume, SFXVolume, AmbienceVolume or UIVolume.
/// </summary>
[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    [Tooltip("Must match an exposed parameter name in the Audio Mixer.")]
    [SerializeField] private string parameter = AudioManager.MasterParam;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    // Runs every time the settings menu is shown, so the slider always reflects the saved value.
    private void OnEnable()
    {
        if (AudioManager.Instance != null)
            slider.SetValueWithoutNotify(AudioManager.Instance.GetVolume(parameter));
    }

    private void OnSliderChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetVolume(parameter, value);
    }
}