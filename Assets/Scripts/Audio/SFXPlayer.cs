using UnityEngine;
using UnityEngine.Audio;

namespace ShieldWall.Audio
{
    public class SFXPlayer : MonoBehaviour
    {
        private AudioSource _audioSource;
        private AudioMixerGroup _mixerGroup;

        public void Initialize(AudioMixerGroup mixerGroup)
        {
            _mixerGroup = mixerGroup;
            SetupAudioSource();
        }

        private void SetupAudioSource()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = _mixerGroup;
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
        }

        public void PlayOneShot(AudioClip clip)
        {
            if (clip != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        public void PlayOneShot(AudioClip clip, float volumeScale)
        {
            if (clip != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(clip, volumeScale);
            }
        }

        public void PlayRandomPitch(AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f)
        {
            if (clip == null || _audioSource == null) return;

            float originalPitch = _audioSource.pitch;
            _audioSource.pitch = Random.Range(minPitch, maxPitch);
            _audioSource.PlayOneShot(clip);
            _audioSource.pitch = originalPitch;
        }
    }
}

