using UnityEngine;
using UnityEngine.Audio;
using ShieldWall.Core;
using ShieldWall.Dice;
using ShieldWall.Data;

namespace ShieldWall.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Mixer")]
        [SerializeField] private AudioMixer _masterMixer;
        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private AudioMixerGroup _sfxGroup;
        [SerializeField] private AudioMixerGroup _ambientGroup;

        [Header("SFX - Dice")]
        [SerializeField] private AudioClip _diceRoll;
        [SerializeField] private AudioClip _diceLock;

        [Header("SFX - Combat")]
        [SerializeField] private AudioClip _hit;
        [SerializeField] private AudioClip _block;
        [SerializeField] private AudioClip _kill;

        [Header("SFX - UI")]
        [SerializeField] private AudioClip _buttonClick;
        [SerializeField] private AudioClip _turnStart;

        [Header("Music")]
        [SerializeField] private AudioClip _battleMusic;
        [SerializeField] private AudioClip _menuMusic;

        [Header("Ambient")]
        [SerializeField] private AudioClip _ambientWind;

        private SFXPlayer _sfxPlayer;
        private MusicPlayer _musicPlayer;
        private AudioSource _ambientSource;

        private const string MUSIC_VOLUME_PARAM = "MusicVolume";
        private const string SFX_VOLUME_PARAM = "SFXVolume";
        private const string AMBIENT_VOLUME_PARAM = "AmbientVolume";

        private const float DEFAULT_MUSIC_VOLUME = 0.5f;
        private const float DEFAULT_SFX_VOLUME = 0.8f;
        private const float DEFAULT_AMBIENT_VOLUME = 0.3f;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupAudioComponents();
            LoadVolumeSettings();
        }

        void OnEnable()
        {
            SubscribeToEvents();
        }

        void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void SetupAudioComponents()
        {
            _sfxPlayer = gameObject.AddComponent<SFXPlayer>();
            _sfxPlayer.Initialize(_sfxGroup);

            _musicPlayer = gameObject.AddComponent<MusicPlayer>();
            _musicPlayer.Initialize(_musicGroup);

            _ambientSource = gameObject.AddComponent<AudioSource>();
            _ambientSource.outputAudioMixerGroup = _ambientGroup;
            _ambientSource.loop = true;
            _ambientSource.playOnAwake = false;
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnDiceRolled += HandleDiceRolled;
            GameEvents.OnDieLockToggled += HandleDieLockToggled;
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            GameEvents.OnAttackBlocked += HandleAttackBlocked;
            GameEvents.OnAttackLanded += HandleAttackLanded;
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
            GameEvents.OnBrotherDied += HandleBrotherDied;
            GameEvents.OnPlayerWounded += HandlePlayerWounded;
            GameEvents.OnBattleEnded += HandleBattleEnded;
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.OnDiceRolled -= HandleDiceRolled;
            GameEvents.OnDieLockToggled -= HandleDieLockToggled;
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            GameEvents.OnAttackBlocked -= HandleAttackBlocked;
            GameEvents.OnAttackLanded -= HandleAttackLanded;
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
            GameEvents.OnBrotherDied -= HandleBrotherDied;
            GameEvents.OnPlayerWounded -= HandlePlayerWounded;
            GameEvents.OnBattleEnded -= HandleBattleEnded;
        }

        private void HandleDiceRolled(RuneDie[] dice)
        {
            PlaySFX(_diceRoll);
        }

        private void HandleDieLockToggled(int index, bool isLocked)
        {
            PlaySFX(_diceLock);
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            PlaySFX(_kill);
        }

        private void HandleAttackBlocked(Attack attack)
        {
            PlaySFX(_block);
        }

        private void HandleAttackLanded(Attack attack)
        {
            PlaySFX(_hit);
        }

        private void HandlePhaseChanged(TurnPhase phase)
        {
            if (phase == TurnPhase.PlayerTurn)
            {
                PlaySFX(_turnStart);
            }
        }

        private void HandleBrotherDied(ShieldBrotherSO brother)
        {
            PlaySFX(_kill);
        }

        private void HandlePlayerWounded(int damage)
        {
            PlaySFX(_hit);
        }

        private void HandleBattleEnded(bool victory)
        {
            StopAmbient();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip != null && _sfxPlayer != null)
            {
                _sfxPlayer.PlayOneShot(clip);
            }
        }

        public void PlaySFX(AudioClip clip, float volumeScale)
        {
            if (clip != null && _sfxPlayer != null)
            {
                _sfxPlayer.PlayOneShot(clip, volumeScale);
            }
        }

        public void PlayButtonClick()
        {
            PlaySFX(_buttonClick);
        }

        public void PlayBattleMusic()
        {
            if (_musicPlayer != null)
            {
                _musicPlayer.Play(_battleMusic);
            }
        }

        public void PlayMenuMusic()
        {
            if (_musicPlayer != null)
            {
                _musicPlayer.Play(_menuMusic);
            }
        }

        public void StopMusic()
        {
            if (_musicPlayer != null)
            {
                _musicPlayer.Stop();
            }
        }

        public void CrossfadeToMusic(AudioClip clip, float duration = 1f)
        {
            if (_musicPlayer != null)
            {
                _musicPlayer.CrossfadeTo(clip, duration);
            }
        }

        public void PlayAmbient()
        {
            if (_ambientWind != null && _ambientSource != null)
            {
                _ambientSource.clip = _ambientWind;
                _ambientSource.Play();
            }
        }

        public void StopAmbient()
        {
            if (_ambientSource != null)
            {
                _ambientSource.Stop();
            }
        }

        public void SetMusicVolume(float normalizedVolume)
        {
            float dbVolume = NormalizedToDecibels(normalizedVolume);
            _masterMixer?.SetFloat(MUSIC_VOLUME_PARAM, dbVolume);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_PARAM, normalizedVolume);
        }

        public void SetSFXVolume(float normalizedVolume)
        {
            float dbVolume = NormalizedToDecibels(normalizedVolume);
            _masterMixer?.SetFloat(SFX_VOLUME_PARAM, dbVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME_PARAM, normalizedVolume);
        }

        public void SetAmbientVolume(float normalizedVolume)
        {
            float dbVolume = NormalizedToDecibels(normalizedVolume);
            _masterMixer?.SetFloat(AMBIENT_VOLUME_PARAM, dbVolume);
            PlayerPrefs.SetFloat(AMBIENT_VOLUME_PARAM, normalizedVolume);
        }

        public float GetMusicVolume()
        {
            return PlayerPrefs.GetFloat(MUSIC_VOLUME_PARAM, DEFAULT_MUSIC_VOLUME);
        }

        public float GetSFXVolume()
        {
            return PlayerPrefs.GetFloat(SFX_VOLUME_PARAM, DEFAULT_SFX_VOLUME);
        }

        public float GetAmbientVolume()
        {
            return PlayerPrefs.GetFloat(AMBIENT_VOLUME_PARAM, DEFAULT_AMBIENT_VOLUME);
        }

        private void LoadVolumeSettings()
        {
            SetMusicVolume(GetMusicVolume());
            SetSFXVolume(GetSFXVolume());
            SetAmbientVolume(GetAmbientVolume());
        }

        private float NormalizedToDecibels(float normalized)
        {
            if (normalized <= 0f) return -80f;
            return Mathf.Log10(normalized) * 20f;
        }
    }
}





