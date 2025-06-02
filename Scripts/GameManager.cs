using _02.Scripts.Resource;
using System.Linq.Expressions;
using UnityEngine;

namespace _02.Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private GunData Rifledamage;
        [SerializeField] private GunData MucinDamage;
        [SerializeField] private SaveShipData shipData;
        [SerializeField] private ResourceInventory inventory;
        private UpgradeButton upgradeButton;

        [SerializeField] private float farmingDuration;
        [SerializeField] private float defenseDuration;
        public float DefenseDuration
        {
            get { return defenseDuration; }
        }

        [SerializeField] public TimerController timerController;
        public int currentKillCount;
        public int totalSpawned;

        private int stageIndex = 1;
        public int StageIndex => stageIndex;
        GameState gameState = GameState.Lobby;
        public void SetStageIndex(int value)
        {
            stageIndex = value;
        }
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        // private void Start()
        // {
        //     InitStage();
        // }

        public void InitStage()
        {
            EventBus.Raise(new StartGame());
            timerController.OnTimerEnd += TimerEndEvent;
            StartFarmingPhase();
        }

        private void StartFarmingPhase()
        {
            Debug.Log("StartFarmingPhase");
            gameState = GameState.Farm;
            timerController.SetTime(gameState, farmingDuration);
            EventBus.Raise(new FarmingPhaseStarted());
        }

        private void EndFarmingPhase()
        {
            Debug.Log("EndFarmingPhase");
            EventBus.Raise(new FarmingPhaseEnded());
            StartDefensePhase();
        }

        private void StartDefensePhase()
        {
            Debug.Log("StartDefensePhase");
            gameState = GameState.Defense;
            timerController.SetTime(gameState, defenseDuration);
            EventBus.Raise(new DefensePhaseStarted(stageIndex++));
        }

        private void EndDefensePhase()
        {
            Debug.Log("EndDefensePhase");
            // 모든몬스터 숨김처리로 풀하기 호출.
            EventBus.Raise(new DefensePhaseEnded());
            // 시간초가 다되면 결과창 띄우기.
            EventBus.Raise(new DefenseResultShowRequested(stageIndex, currentKillCount, totalSpawned));
            SaveManager.SaveGame(Rifledamage, MucinDamage, shipData, inventory, upgradeButton, this); // 자동 저장
            StartFarmingPhase();

        }

        private void TimerEndEvent(GameState gameStat, float time)
        {
            if (gameStat == GameState.Farm)
            {
                gameState = GameState.Defense;
                timerController.SetTime(gameState, defenseDuration);
                EndFarmingPhase();
            }
            else if (gameStat == GameState.Defense)
            {
                gameState = GameState.Farm;
                timerController.SetTime(gameState, farmingDuration);
                EndDefensePhase();
            }
        }

        public void InitGameSceneObjects(ResourceInventory inventory, UpgradeButton upgradeButton)
        {
            this.inventory = inventory;
            this.upgradeButton = upgradeButton;
        }
    }
}