using UnityEngine;
using UnityEngine.UI;

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