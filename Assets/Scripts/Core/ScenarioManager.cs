using UnityEngine;
using ShieldWall.Data;

namespace ShieldWall.Core
{
    public class ScenarioManager : MonoBehaviour
    {
        public static ScenarioManager Instance { get; private set; }

        [SerializeField] private BattleScenarioSO _defaultScenario;

        private BattleScenarioSO _selectedScenario;

        public BattleScenarioSO SelectedScenario => _selectedScenario ?? _defaultScenario;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SelectScenario(BattleScenarioSO scenario)
        {
            _selectedScenario = scenario;
        }

        public void LoadDefaultScenario()
        {
            _selectedScenario = _defaultScenario;
        }

        public void ClearSelection()
        {
            _selectedScenario = null;
        }
    }
}








