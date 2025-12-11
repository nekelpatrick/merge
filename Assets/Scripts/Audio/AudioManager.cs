using UnityEngine;
using UnityEngine.Audio;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _musicSource;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip _diceRollClip;
        [SerializeField] private AudioClip _diceSelectClip;
        [SerializeField] private AudioClip _hitClip;
        [SerializeField] private AudioClip _blockClip;
        [SerializeField] private AudioClip _deathClip;
        [SerializeField] private AudioClip _victoryClip;
        [SerializeField] private AudioClip _defeatClip;
        [SerializeField] private AudioClip _buttonClickClip;

        [Header("Volume Settings")]
        [SerializeField] [Range(0f, 1f)] private float _sfxVolume = 0.8f;
        [SerializeField] [Range(0f, 1f)] private float _musicVolume = 0.5f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupAudioSources();
        }

        private void OnEnable()
        {
            GameEvents.OnDiceRolled += HandleDiceRolled;
            GameEvents.OnDieLockToggled += HandleDieLockToggled;
            GameEvents.OnPlayerWounded += HandlePlayerWounded;
            GameEvents.OnBrotherWounded += HandleBrotherWounded;
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            GameEvents.OnAttackBlocked += HandleAttackBlocked;
            GameEvents.OnBrotherDied += HandleBrotherDied;
            GameEvents.OnBattleEnded += HandleBattleEnded;
        }

        private void OnDisable()
        {
            GameEvents.OnDiceRolled -= HandleDiceRolled;
            GameEvents.OnDieLockToggled -= HandleDieLockToggled;
            GameEvents.OnPlayerWounded -= HandlePlayerWounded;
            GameEvents.OnBrotherWounded -= HandleBrotherWounded;
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            GameEvents.OnAttackBlocked -= HandleAttackBlocked;
            GameEvents.OnBrotherDied -= HandleBrotherDied;
            GameEvents.OnBattleEnded -= HandleBattleEnded;
        }

        private void SetupAudioSources()
        {
            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
            }

            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.playOnAwake = false;
                _musicSource.loop = true;
            }

            _sfxSource.volume = _sfxVolume;
            _musicSource.volume = _musicVolume;
        }

        private void HandleDiceRolled(Dice.RuneDie[] dice)
        {
            PlaySFX(_diceRollClip);
        }

        private void HandleDieLockToggled(int index, bool isLocked)
        {
            PlaySFX(_diceSelectClip, 0.5f);
        }

        private void HandlePlayerWounded(int damage)
        {
            PlaySFX(_hitClip);
        }

        private void HandleBrotherWounded(ShieldBrotherSO brother, int damage)
        {
            PlaySFX(_hitClip, 0.7f);
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            PlaySFX(_deathClip);
        }

        private void HandleAttackBlocked(Attack attack)
        {
            PlaySFX(_blockClip);
        }

        private void HandleBrotherDied(ShieldBrotherSO brother)
        {
            PlaySFX(_deathClip);
        }

        private void HandleBattleEnded(bool victory)
        {
            if (victory)
            {
                PlaySFX(_victoryClip);
            }
            else
            {
                PlaySFX(_defeatClip);
            }
        }

        public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null || _sfxSource == null) return;
            _sfxSource.PlayOneShot(clip, _sfxVolume * volumeMultiplier);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (_musicSource == null) return;
            
            _musicSource.clip = clip;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            if (_musicSource != null)
            {
                _musicSource.Stop();
            }
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = Mathf.Clamp01(volume);
            if (_sfxSource != null)
            {
                _sfxSource.volume = _sfxVolume;
            }
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = Mathf.Clamp01(volume);
            if (_musicSource != null)
            {
                _musicSource.volume = _musicVolume;
            }
        }

        public void PlayButtonClick()
        {
            PlaySFX(_buttonClickClip, 0.5f);
        }
    }
}

