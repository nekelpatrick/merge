using UnityEngine;
using TMPro;
using ShieldWall.Core;
using ShieldWall.Combat;

namespace ShieldWall.UI
{
    public class WaveUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _waveText;
        [SerializeField] private EnemyWaveController _waveController;

        [Header("Format")]
        [SerializeField] private string _waveFormat = "Wave {0}/{1}";

        private void OnEnable()
        {
            GameEvents.OnWaveStarted += HandleWaveStarted;
        }

        private void OnDisable()
        {
            GameEvents.OnWaveStarted -= HandleWaveStarted;
        }

        private void Start()
        {
            UpdateDisplay();
        }

        private void HandleWaveStarted(int waveNumber)
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (_waveText == null) return;

            int current = _waveController != null ? _waveController.CurrentWaveNumber : 1;
            int total = _waveController != null ? _waveController.TotalWaves : 3;

            _waveText.text = string.Format(_waveFormat, current, total);
        }
    }
}

