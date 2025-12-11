using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using ShieldWall.Core;

namespace ShieldWall.Audio
{
    public class AmbientController : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _ambientSource;
        [SerializeField] private AudioSource _crowdSource;

        [Header("Ambient Clips")]
        [SerializeField] private AudioClip _battleAmbience;
        [SerializeField] private AudioClip _windAmbience;

        [Header("Crowd Clips")]
        [SerializeField] private AudioClip _crowdMurmur;
        [SerializeField] private AudioClip _crowdCheer;
        [SerializeField] private AudioClip _crowdGasp;

        [Header("Settings")]
        [SerializeField] private float _ambientVolume = 0.3f;
        [SerializeField] private float _crowdVolume = 0.2f;
        [SerializeField] private float _fadeDuration = 1f;

        [Header("Mixer")]
        [SerializeField] private AudioMixerGroup _mixerGroup;

        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            SetupAudioSources();
        }

        private void OnEnable()
        {
            GameEvents.OnWaveStarted += HandleWaveStarted;
            GameEvents.OnWaveCleared += HandleWaveCleared;
            GameEvents.OnBattleEnded += HandleBattleEnded;
            GameEvents.OnBrotherDied += HandleBrotherDied;
        }

        private void OnDisable()
        {
            GameEvents.OnWaveStarted -= HandleWaveStarted;
            GameEvents.OnWaveCleared -= HandleWaveCleared;
            GameEvents.OnBattleEnded -= HandleBattleEnded;
            GameEvents.OnBrotherDied -= HandleBrotherDied;
        }

        private void SetupAudioSources()
        {
            if (_ambientSource == null)
            {
                var ambientGO = new GameObject("AmbientSource");
                ambientGO.transform.SetParent(transform);
                _ambientSource = ambientGO.AddComponent<AudioSource>();
            }

            if (_crowdSource == null)
            {
                var crowdGO = new GameObject("CrowdSource");
                crowdGO.transform.SetParent(transform);
                _crowdSource = crowdGO.AddComponent<AudioSource>();
            }

            ConfigureSource(_ambientSource);
            ConfigureSource(_crowdSource);
        }

        private void ConfigureSource(AudioSource source)
        {
            source.outputAudioMixerGroup = _mixerGroup;
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 0f;
        }

        public void StartBattleAmbience()
        {
            if (_battleAmbience != null)
            {
                _ambientSource.clip = _battleAmbience;
                _ambientSource.volume = 0f;
                _ambientSource.Play();
                FadeSource(_ambientSource, _ambientVolume);
            }

            if (_crowdMurmur != null)
            {
                _crowdSource.clip = _crowdMurmur;
                _crowdSource.volume = 0f;
                _crowdSource.Play();
                FadeSource(_crowdSource, _crowdVolume);
            }
        }

        public void StopAmbience()
        {
            FadeSource(_ambientSource, 0f, () => _ambientSource.Stop());
            FadeSource(_crowdSource, 0f, () => _crowdSource.Stop());
        }

        private void HandleWaveStarted(int waveNumber)
        {
            FadeSource(_ambientSource, _ambientVolume * 1.2f);
        }

        private void HandleWaveCleared()
        {
            PlayCrowdCheer();
            FadeSource(_ambientSource, _ambientVolume * 0.8f);
        }

        private void HandleBattleEnded(bool victory)
        {
            if (victory)
                PlayCrowdCheer();

            StopAmbience();
        }

        private void HandleBrotherDied(Data.ShieldBrotherSO brother)
        {
            PlayCrowdGasp();
        }

        public void PlayCrowdCheer()
        {
            if (_crowdCheer != null)
                StartCoroutine(PlayOneShotOnCrowd(_crowdCheer));
        }

        public void PlayCrowdGasp()
        {
            if (_crowdGasp != null)
                StartCoroutine(PlayOneShotOnCrowd(_crowdGasp));
        }

        private IEnumerator PlayOneShotOnCrowd(AudioClip clip)
        {
            float originalVolume = _crowdSource.volume;
            _crowdSource.volume = originalVolume * 0.5f;

            var tempSource = gameObject.AddComponent<AudioSource>();
            tempSource.outputAudioMixerGroup = _mixerGroup;
            tempSource.clip = clip;
            tempSource.volume = _crowdVolume;
            tempSource.Play();

            yield return new WaitForSeconds(clip.length);

            Destroy(tempSource);
            _crowdSource.volume = originalVolume;
        }

        private void FadeSource(AudioSource source, float targetVolume, System.Action onComplete = null)
        {
            StartCoroutine(FadeRoutine(source, targetVolume, onComplete));
        }

        private IEnumerator FadeRoutine(AudioSource source, float targetVolume, System.Action onComplete)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < _fadeDuration)
            {
                float t = elapsed / _fadeDuration;
                source.volume = Mathf.Lerp(startVolume, targetVolume, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            source.volume = targetVolume;
            onComplete?.Invoke();
        }
    }
}

