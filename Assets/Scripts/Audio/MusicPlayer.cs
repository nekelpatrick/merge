using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace ShieldWall.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource _sourceA;
        private AudioSource _sourceB;
        private AudioSource _activeSource;
        private AudioMixerGroup _mixerGroup;
        private Coroutine _crossfadeCoroutine;

        public void Initialize(AudioMixerGroup mixerGroup)
        {
            _mixerGroup = mixerGroup;
            SetupAudioSources();
        }

        private void SetupAudioSources()
        {
            _sourceA = gameObject.AddComponent<AudioSource>();
            _sourceA.outputAudioMixerGroup = _mixerGroup;
            _sourceA.playOnAwake = false;
            _sourceA.loop = true;
            _sourceA.spatialBlend = 0f;

            _sourceB = gameObject.AddComponent<AudioSource>();
            _sourceB.outputAudioMixerGroup = _mixerGroup;
            _sourceB.playOnAwake = false;
            _sourceB.loop = true;
            _sourceB.spatialBlend = 0f;

            _activeSource = _sourceA;
        }

        public void Play(AudioClip clip)
        {
            if (clip == null) return;

            if (_activeSource.clip == clip && _activeSource.isPlaying)
                return;

            _activeSource.clip = clip;
            _activeSource.volume = 1f;
            _activeSource.Play();
        }

        public void Stop()
        {
            if (_crossfadeCoroutine != null)
            {
                StopCoroutine(_crossfadeCoroutine);
                _crossfadeCoroutine = null;
            }

            _sourceA.Stop();
            _sourceB.Stop();
        }

        public void CrossfadeTo(AudioClip clip, float duration)
        {
            if (clip == null) return;

            if (_activeSource.clip == clip && _activeSource.isPlaying)
                return;

            if (_crossfadeCoroutine != null)
            {
                StopCoroutine(_crossfadeCoroutine);
            }

            _crossfadeCoroutine = StartCoroutine(CrossfadeCoroutine(clip, duration));
        }

        private IEnumerator CrossfadeCoroutine(AudioClip newClip, float duration)
        {
            AudioSource fadeOutSource = _activeSource;
            AudioSource fadeInSource = _activeSource == _sourceA ? _sourceB : _sourceA;

            fadeInSource.clip = newClip;
            fadeInSource.volume = 0f;
            fadeInSource.Play();

            float elapsed = 0f;
            float startVolume = fadeOutSource.volume;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                fadeOutSource.volume = Mathf.Lerp(startVolume, 0f, t);
                fadeInSource.volume = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }

            fadeOutSource.Stop();
            fadeOutSource.volume = 0f;
            fadeInSource.volume = 1f;

            _activeSource = fadeInSource;
            _crossfadeCoroutine = null;
        }

        public bool IsPlaying => _activeSource != null && _activeSource.isPlaying;
    }
}

