using FrogGame._Core.Common;
using FrogGame._Core.Common.CoreEnums;
using FrogGame._Core.EventService;
using FrogGame._Core.EventService.CoreEvents;
using FrogGame.Common.Events;
using UnityEngine;

namespace FrogGame._Core.Managers
{
    public class GameManager : SingleMonoBehaviour<GameManager>
    {
        public int MoveCount { get { return _moveCount; } }

        private GameState _currentGameState;
        private int _moveCount;

        private GameStateChanged _gameStateChanged = new();
        private UpdateUIGamePanel _updateUIGamePanel = new();

        private void Start()
        {
            ChangeGameState(GameState.Menu);
        }

        private void OnEnable()
        {
            GameEventService.On<StartLevel>(OnStartLevel);
            GameEventService.On<FinishLevel>(OnFinishLevel);
        }

        private void OnDisable()
        {
            GameEventService.Off<StartLevel>(OnStartLevel);
            GameEventService.Off<FinishLevel>(OnFinishLevel);
        }

        private void OnFinishLevel(FinishLevel e)
        {
            switch (e.FinishGameState)
            {
                case GameState.Complete:
                    CompleteLevel();
                    break;
                case GameState.Fail:
                    FailLevel();
                    break;
            }
        }

        private void ChangeGameState(GameState gameState)
        {
            _currentGameState = gameState;
            _gameStateChanged.GameState = _currentGameState;
            GameEventService.Fire(_gameStateChanged);
        }

        private void OnStartLevel(StartLevel e)
        {
            _moveCount = e.MoveCount;
            PlayLevel();
            UpdateMoveCount(0);
        }

        private void PlayLevel()
        {
            ChangeGameState(GameState.Play);
            FireEnablePlayerInputEvent();
        }

        private void CompleteLevel()
        {
            FireDisablePlayerInputEvent();
            IterateLevelIndex();
            ChangeGameState(GameState.Complete);
        }

        private void FailLevel()
        {
            FireDisablePlayerInputEvent();
            ChangeGameState(GameState.Fail);
        }

        private void UpdateMoveCount(int count)
        {
            _moveCount += count;
            FireUpdateUIGamePanelEvent();
        }

        private void IterateLevelIndex()
        {
            var levelIndex = PlayerPrefs.GetInt(GlobalConstants.CURRENT_LEVEL_KEY);
            levelIndex++;
            PlayerPrefs.SetInt(GlobalConstants.CURRENT_LEVEL_KEY, levelIndex);
        }
        private void FireUpdateUIGamePanelEvent()
        {
            _updateUIGamePanel.MoveCount = _moveCount;
            GameEventService.Fire(_updateUIGamePanel);
        }

        private void FireEnablePlayerInputEvent() => GameEventService.Fire(new EnablePlayerInput());

        private void FireDisablePlayerInputEvent() => GameEventService.Fire(new DisablePlayerInput());

        public bool GetMovePermit()
        {
            if (_moveCount == 0) return false;

            UpdateMoveCount(-1);
            return true;
        }
    }
}
