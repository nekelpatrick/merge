using UnityEngine;
using ShieldWall.Core;
using ShieldWall.Data;

namespace ShieldWall.Audio
{
    public class CombatSFXController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SFXPlayer _sfxPlayer;

        [Header("Hit Sounds")]
        [SerializeField] private AudioClip[] _hitSounds;
        [SerializeField] private AudioClip[] _criticalHitSounds;

        [Header("Block Sounds")]
        [SerializeField] private AudioClip[] _blockSounds;
        [SerializeField] private AudioClip _shieldBashSound;

        [Header("Kill Sounds")]
        [SerializeField] private AudioClip[] _enemyDeathSounds;
        [SerializeField] private AudioClip[] _brotherDeathSounds;

        [Header("Wound Sounds")]
        [SerializeField] private AudioClip[] _playerWoundSounds;
        [SerializeField] private AudioClip[] _brotherWoundSounds;

        [Header("Wave Sounds")]
        [SerializeField] private AudioClip _waveStartSound;
        [SerializeField] private AudioClip _waveClearSound;
        [SerializeField] private AudioClip _victorySound;
        [SerializeField] private AudioClip _defeatSound;

        private void OnEnable()
        {
            GameEvents.OnPlayerWounded += HandlePlayerWounded;
            GameEvents.OnBrotherWounded += HandleBrotherWounded;
            GameEvents.OnBrotherDied += HandleBrotherDied;
            GameEvents.OnEnemyKilled += HandleEnemyKilled;
            GameEvents.OnAttackBlocked += HandleAttackBlocked;
            GameEvents.OnWaveStarted += HandleWaveStarted;
            GameEvents.OnWaveCleared += HandleWaveCleared;
            GameEvents.OnBattleEnded += HandleBattleEnded;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerWounded -= HandlePlayerWounded;
            GameEvents.OnBrotherWounded -= HandleBrotherWounded;
            GameEvents.OnBrotherDied -= HandleBrotherDied;
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
            GameEvents.OnAttackBlocked -= HandleAttackBlocked;
            GameEvents.OnWaveStarted -= HandleWaveStarted;
            GameEvents.OnWaveCleared -= HandleWaveCleared;
            GameEvents.OnBattleEnded -= HandleBattleEnded;
        }

        private void HandlePlayerWounded(int damage)
        {
            if (damage >= 2 && _criticalHitSounds.Length > 0)
                _sfxPlayer?.PlayRandom(_criticalHitSounds);
            else if (_playerWoundSounds.Length > 0)
                _sfxPlayer?.PlayRandom(_playerWoundSounds);
            else if (_hitSounds.Length > 0)
                _sfxPlayer?.PlayRandom(_hitSounds);
        }

        private void HandleBrotherWounded(ShieldBrotherSO brother, int damage)
        {
            if (_brotherWoundSounds.Length > 0)
                _sfxPlayer?.PlayRandom(_brotherWoundSounds);
            else if (_hitSounds.Length > 0)
                _sfxPlayer?.PlayRandom(_hitSounds);
        }

        private void HandleBrotherDied(ShieldBrotherSO brother)
        {
            if (_brotherDeathSounds.Length > 0)
                _sfxPlayer?.PlayRandom(_brotherDeathSounds);
        }

        private void HandleEnemyKilled(EnemySO enemy)
        {
            if (_enemyDeathSounds.Length > 0)
                _sfxPlayer?.PlayRandom(_enemyDeathSounds);
        }

        private void HandleAttackBlocked(Attack attack)
        {
            if (_blockSounds.Length > 0)
                _sfxPlayer?.PlayRandom(_blockSounds);
        }

        private void HandleWaveStarted(int waveNumber)
        {
            if (_waveStartSound != null)
                _sfxPlayer?.PlayOneShot(_waveStartSound);
        }

        private void HandleWaveCleared()
        {
            if (_waveClearSound != null)
                _sfxPlayer?.PlayOneShot(_waveClearSound);
        }

        private void HandleBattleEnded(bool victory)
        {
            var clip = victory ? _victorySound : _defeatSound;
            if (clip != null)
                _sfxPlayer?.PlayOneShot(clip);
        }

        public void PlayShieldBash()
        {
            if (_shieldBashSound != null)
                _sfxPlayer?.PlayRandomPitch(_shieldBashSound);
        }
    }
}

