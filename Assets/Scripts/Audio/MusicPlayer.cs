using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace ShieldWall.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        private AudioSource _sourceA;
        private AudioSource _sourceB;
        private AudioMixerGroup _mixerGroup;
        private bool _isSourceAActive = true;
        private Coroutine _crossfadeCoroutine;

        public AudioClip CurrentClip => ActiveSource.clip;
        public bool IsPlaying => ActiveSource.isPlaying;
        private AudioSource ActiveSource => _isSourceAActive ? _sourceA : _sourceB;
        private AudioSource InactiveSource => _isSourceAActive ? _sourceB : _sourceA;

        public void Initialize(AudioMixerGroup mixerGroup)
        {
            _mixerGroup = mixerGroup;
            SetupAudioSources();
        }

        private void SetupAudioSources()
        {
            _sourceA = gameObject.AddComponent<AudioSource>();
            _sourceA.outputAudioMixerGroup = _mixerGroup;
            _sourceA.loop = true;
            _sourceA.playOnAwake = false;

            _sourceB = gameObject.AddComponent<AudioSource>();
            _sourceB.outputAudioMixerGroup = _mixerGroup;
            _sourceB.loop = true;
            _sourceB.playOnAwake = false;
            _sourceB.volume = 0f;
        }

        public void Play(AudioClip clip)
        {
            if (clip == null) return;

            if (ActiveSource.clip == clip && ActiveSource.isPlaying)
                return;

            StopCrossfade();

            ActiveSource.clip = clip;
            ActiveSource.volume = 1f;
            ActiveSource.Play();
        }

        public void Stop()
        {
            StopCrossfade();
            _sourceA?.Stop();
            _sourceB?.Stop();
        }

        public void CrossfadeTo(AudioClip clip, float duration = 1f)
        {
            if (clip == null) return;

            if (ActiveSource.clip == clip && ActiveSource.isPlaying)
                return;

            StopCrossfade();
            _crossfadeCoroutine = StartCoroutine(CrossfadeCoroutine(clip, duration));
        }

        private void StopCrossfade()
        {
            if (_crossfadeCoroutine != null)
            {
                StopCoroutine(_crossfadeCoroutine);
                _crossfadeCoroutine = null;
            }
        }

        private IEnumerator CrossfadeCoroutine(AudioClip newClip, float duration)
        {
            var fadeOut = ActiveSource;
            var fadeIn = InactiveSource;

            fadeIn.clip = newClip;
            fadeIn.volume = 0f;
            fadeIn.Play();

            float elapsed = 0f;
            float startVolume = fadeOut.volume;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                fadeOut.volume = Mathf.Lerp(startVolume, 0f, t);
                fadeIn.volume = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }

            fadeOut.Stop();
            fadeOut.volume = 0f;
            fadeIn.volume = 1f;

            _isSourceAActive = !_isSourceAActive;
            _crossfadeCoroutine = null;
        }

        public void SetVolume(float volume)
        {
            if (_sourceA != null) _sourceA.volume = volume;
            if (_sourceB != null) _sourceB.volume = _sourceB.isPlaying ? volume : 0f;
        }
    }
}
