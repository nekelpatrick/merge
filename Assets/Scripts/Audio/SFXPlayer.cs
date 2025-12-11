using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace ShieldWall.Audio
{
    public class SFXPlayer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _maxSimultaneousSounds = 8;
        [SerializeField] private float _minPitchVariation = 0.9f;
        [SerializeField] private float _maxPitchVariation = 1.1f;

        private List<AudioSource> _audioSources = new List<AudioSource>();
        private AudioMixerGroup _mixerGroup;
        private int _currentSourceIndex;
        private Dictionary<AudioClip, float> _lastPlayTime = new Dictionary<AudioClip, float>();
        private const float MIN_REPEAT_DELAY = 0.05f;

        public void Initialize(AudioMixerGroup mixerGroup)
        {
            _mixerGroup = mixerGroup;
            SetupAudioSources();
        }

        private void SetupAudioSources()
        {
            for (int i = 0; i < _maxSimultaneousSounds; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = _mixerGroup;
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                _audioSources.Add(source);
            }
        }

        private AudioSource GetNextSource()
        {
            if (_audioSources.Count == 0) return null;
            var source = _audioSources[_currentSourceIndex];
            _currentSourceIndex = (_currentSourceIndex + 1) % _audioSources.Count;
            return source;
        }

        public void PlayOneShot(AudioClip clip)
        {
            PlayOneShot(clip, 1f);
        }

        public void PlayOneShot(AudioClip clip, float volumeScale)
        {
            if (clip == null) return;
            if (!CanPlayClip(clip)) return;

            var source = GetNextSource();
            if (source != null)
            {
                source.pitch = 1f;
                source.PlayOneShot(clip, volumeScale);
                _lastPlayTime[clip] = Time.unscaledTime;
            }
        }

        public void PlayRandomPitch(AudioClip clip, float minPitch, float maxPitch)
        {
            if (clip == null) return;
            if (!CanPlayClip(clip)) return;

            var source = GetNextSource();
            if (source != null)
            {
                source.pitch = Random.Range(minPitch, maxPitch);
                source.PlayOneShot(clip);
                _lastPlayTime[clip] = Time.unscaledTime;
            }
        }

        public void PlayRandomPitch(AudioClip clip)
        {
            PlayRandomPitch(clip, _minPitchVariation, _maxPitchVariation);
        }

        public void PlayRandom(AudioClip[] clips, float volumeScale = 1f, bool randomPitch = true)
        {
            if (clips == null || clips.Length == 0) return;

            var clip = clips[Random.Range(0, clips.Length)];
            if (randomPitch)
                PlayRandomPitch(clip);
            else
                PlayOneShot(clip, volumeScale);
        }

        private bool CanPlayClip(AudioClip clip)
        {
            if (_lastPlayTime.TryGetValue(clip, out float lastTime))
            {
                return Time.unscaledTime - lastTime >= MIN_REPEAT_DELAY;
            }
            return true;
        }

        public void StopAll()
        {
            foreach (var source in _audioSources)
            {
                source.Stop();
            }
        }
    }
}

