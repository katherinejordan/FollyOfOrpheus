using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] private SoundData sound;

    public void PlaySound()
    {
        AudioManager.Instance?.Play(sound);
    }
}