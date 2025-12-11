using UnityEngine;
using ShieldWall.Core;

namespace ShieldWall.Audio
{
    public class UISFXController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SFXPlayer _sfxPlayer;

        [Header("Button Sounds")]
        [SerializeField] private AudioClip _buttonHoverSound;
        [SerializeField] private AudioClip _buttonClickSound;
        [SerializeField] private AudioClip _buttonDisabledSound;

        [Header("Dice Sounds")]
        [SerializeField] private AudioClip[] _diceRollSounds;
        [SerializeField] private AudioClip _diceLockSound;
        [SerializeField] private AudioClip _diceUnlockSound;

        [Header("Action Sounds")]
        [SerializeField] private AudioClip _actionSelectSound;
        [SerializeField] private AudioClip _actionConfirmSound;
        [SerializeField] private AudioClip _comboDiscoverySound;

        [Header("Phase Sounds")]
        [SerializeField] private AudioClip _turnStartSound;
        [SerializeField] private AudioClip _turnEndSound;

        [Header("Misc Sounds")]
        [SerializeField] private AudioClip _menuOpenSound;
        [SerializeField] private AudioClip _menuCloseSound;

        private void OnEnable()
        {
            GameEvents.OnDieLockToggled += HandleDieLockToggled;
            GameEvents.OnPhaseChanged += HandlePhaseChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnDieLockToggled -= HandleDieLockToggled;
            GameEvents.OnPhaseChanged -= HandlePhaseChanged;
        }

        private void HandleDieLockToggled(int index, bool locked)
        {
            var clip = locked ? _diceLockSound : _diceUnlockSound;
            if (clip != null)
                _sfxPlayer?.PlayRandomPitch(clip, 0.95f, 1.05f);
        }

        private void HandlePhaseChanged(TurnPhase phase)
        {
            if (phase == TurnPhase.PlayerTurn && _turnStartSound != null)
                _sfxPlayer?.PlayOneShot(_turnStartSound);
        }

        public void PlayButtonHover()
        {
            if (_buttonHoverSound != null)
                _sfxPlayer?.PlayOneShot(_buttonHoverSound, 0.5f);
        }

        public void PlayButtonClick()
        {
            if (_buttonClickSound != null)
                _sfxPlayer?.PlayRandomPitch(_buttonClickSound, 0.95f, 1.05f);
        }

        public void PlayButtonDisabled()
        {
            if (_buttonDisabledSound != null)
                _sfxPlayer?.PlayOneShot(_buttonDisabledSound, 0.3f);
        }

        public void PlayDiceRoll()
        {
            if (_diceRollSounds.Length > 0)
                _sfxPlayer?.PlayRandom(_diceRollSounds);
        }

        public void PlayActionSelect()
        {
            if (_actionSelectSound != null)
                _sfxPlayer?.PlayOneShot(_actionSelectSound);
        }

        public void PlayActionConfirm()
        {
            if (_actionConfirmSound != null)
                _sfxPlayer?.PlayRandomPitch(_actionConfirmSound);
        }

        public void PlayComboDiscovery()
        {
            if (_comboDiscoverySound != null)
                _sfxPlayer?.PlayOneShot(_comboDiscoverySound);
        }

        public void PlayMenuOpen()
        {
            if (_menuOpenSound != null)
                _sfxPlayer?.PlayOneShot(_menuOpenSound);
        }

        public void PlayMenuClose()
        {
            if (_menuCloseSound != null)
                _sfxPlayer?.PlayOneShot(_menuCloseSound);
        }
    }
}

