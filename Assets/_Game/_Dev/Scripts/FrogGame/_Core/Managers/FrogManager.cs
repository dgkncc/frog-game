using FrogGame._Core.Common;
using FrogGame._Core.Common.CoreEnums;
using FrogGame._Core.EventService;
using FrogGame._Core.EventService.CoreEvents;
using FrogGame.Cell.Contents.FrogContent;
using FrogGame.Common.Events;
using FrogGame.ScriptableObjects;
using System.Collections.Generic;
using System.Linq;

namespace FrogGame._Core.Managers
{
    public class FrogManager : SingleMonoBehaviour<FrogManager>
    {
        private List<FrogViewModel> _activeFrogs = new();
        private bool _isLevelPlaying = false;

        private FinishLevel _finishLevel = new();

        private void OnEnable()
        {
            GameEventService.On<GameStateChanged>(OnGameStateChanged);
            GameEventService.On<CheckLevelFail>(OnCheckLevelFail);
            GameEventService.On<RegisterFrog>(OnRegisterFrog);
            GameEventService.On<UnregisterFrog>(OnUnregisterFrog);
        }

        private void OnDisable()
        {
            GameEventService.Off<GameStateChanged>(OnGameStateChanged);
            GameEventService.Off<CheckLevelFail>(OnCheckLevelFail);
            GameEventService.Off<RegisterFrog>(OnRegisterFrog);
            GameEventService.Off<UnregisterFrog>(OnUnregisterFrog);
        }

        private void OnGameStateChanged(GameStateChanged e)
        {
            if (e.GameState == GameState.Complete || e.GameState == GameState.Fail)
                _activeFrogs.Clear();
            else if (e.GameState == GameState.Play)
                _isLevelPlaying = true;
        }


        private void OnCheckLevelFail(CheckLevelFail _)
        {
            CheckForLevelFail();
        }

        private void OnRegisterFrog(RegisterFrog e)
        {
            AddFrog(e.FrogViewModel);
        }

        private void OnUnregisterFrog(UnregisterFrog e)
        {
            RemoveFrog(e.FrogViewModel);
            CheckForLevelCompletion();
        }

        private void AddFrog(FrogViewModel frog)
        {
            _activeFrogs.Add(frog);
        }

        private void RemoveFrog(FrogViewModel frog)
        {
            _activeFrogs.Remove(frog);
        }

        private bool CheckForLevelCompletion()
        {
            if (_activeFrogs.Count == 0 && _isLevelPlaying)
            {
                FinishLevel(GameState.Complete);
                return true;
            }

            return false;
        }

        private void CheckForLevelFail()
        {
            if (_activeFrogs.Any(x => x.FrogState == FrogState.Eating) || !_isLevelPlaying)
                return;

            if (CheckForLevelCompletion())
                return;

            var moveCount = GameManager.Instance.MoveCount;
            if (moveCount == 0)
            {
                FinishLevel(GameState.Fail);
            }
        }

        private void FinishLevel(GameState finishGameState)
        {
            _isLevelPlaying = false;
            _finishLevel.FinishGameState = finishGameState;
            Invoke(nameof(FireFinishLevelEvent), GameSettings.Instance.TongueExtendDuration + GlobalConstants.DELAY_LEVEL_FINISH);
        }

        private void FireFinishLevelEvent()
        {
            GameEventService.Fire(_finishLevel);
        }
    }
}
