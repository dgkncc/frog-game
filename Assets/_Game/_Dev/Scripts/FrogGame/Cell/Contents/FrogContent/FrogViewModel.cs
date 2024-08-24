using FrogGame._Core.EventService;
using FrogGame._Core.Managers;
using FrogGame.Board;
using FrogGame.Common.Enums;
using FrogGame.Common.Events;
using FrogGame.ScriptableObjects;
using System;
using UnityEngine;

namespace FrogGame.Cell.Contents.FrogContent
{
    public class FrogViewModel : CellContentBase
    {
        public FrogState FrogState { get { return _frogState; } }
        public CellContentColor ContentColor { get { return _frogModel.CellContentColor; } }
        public CellContentDirection ContentDirection { get { return _frogModel.CellContentDirection; } }

        [SerializeField] private FrogTongue frogTongue;

        private FrogState _frogState;
        private FrogModel _frogModel;
        private bool _isComplete;
        private bool _isEnabled;

        public event Action OnContentUpdated;
        public event Action OnActivateContent;
        public event Action OnDeactivateContent;
        public event Action OnEatingStart;
        public event Action OnEatingEnd;

        private void OnEnable()
        {
            frogTongue.OnComplete += DisableFrog;
            frogTongue.OnComplete += CompleteFrog;
            frogTongue.OnComplete += EatingStart;
            frogTongue.OnComplete += EatingEnd;
            frogTongue.OnFail += EnableFrog;
            frogTongue.OnFail += EatingEnd;
        }

        private void OnDisable()
        {
            frogTongue.OnComplete -= DisableFrog;
            frogTongue.OnComplete -= CompleteFrog;
            frogTongue.OnComplete -= EatingStart;
            frogTongue.OnComplete -= EatingEnd;
            frogTongue.OnFail -= EnableFrog;
            frogTongue.OnFail -= EatingEnd;
        }

        public override void InitializeContent(CellContentColor contentColor, CellContentDirection contentDirection, int grapeCount, Vector2Int positionInGrid)
        {
            if (_frogModel != null)
            {
                UpdateContent(contentColor, contentDirection, grapeCount, positionInGrid);
            }
            else
            {
                _frogModel = new FrogModel(contentColor, contentDirection, grapeCount, positionInGrid);

            }

            _isComplete = false;
            _frogState = FrogState.Passive;

            frogTongue.Initialize(_frogModel.GrapeCount,
                                  _frogModel.CellContentColor,
                                  _frogModel.CellContentDirection,
                                  _frogModel.PositionInGrid);

            GameEventService.Fire(new RegisterFrog { FrogViewModel = this });
            OnContentUpdated?.Invoke();
        }
        public void UpdateContent(CellContentColor contentColor, CellContentDirection contentDirection, int grapeCount, Vector2Int positionInGrid)
        {
            _frogModel.UpdateContent(contentColor, contentDirection, grapeCount, positionInGrid);
            OnContentUpdated?.Invoke();
        }

        public override void ActivateContent()
        {
            OnActivateContent?.Invoke();
            EnableFrog();
        }

        public override void DeactivateContent()
        {
            OnDeactivateContent?.Invoke();
            DisableFrog();
        }

        private void EnableFrog()
        {
            _isEnabled = true;
        }

        private void DisableFrog()
        {
            _isEnabled = false;
        }

        public void ReceiveInput()
        {
            if (!_isEnabled)
                return;

            ExtendTongue();
        }

        private void ExtendTongue()
        {
            if (!GameManager.Instance.GetMovePermit())
                return;

            DisableFrog();
            frogTongue.ExtendTongue();
            EatingStart();
        }

        private void EatingStart()
        {
            _frogState = FrogState.Eating;
            OnEatingStart?.Invoke();
        }

        private void EatingEnd()
        {
            _frogState = FrogState.Idle;
            OnEatingEnd?.Invoke();
            if (_isComplete)
                UnregisterFrog();
            CheckForLevelFail();
        }

        private void CheckForLevelFail()
        {
            GameEventService.Fire(new CheckLevelFail());
        }

        private void CompleteFrog()
        {
            _frogState = FrogState.Passive;
            var gridNode = GameBoard.Instance.GetNodeInPosition(_frogModel.PositionInGrid.x, _frogModel.PositionInGrid.y);
            gridNode.CompleteActiveCellBlock(GameSettings.Instance.TongueExtendDuration);
            _isComplete = true;
        }

        private void UnregisterFrog()
        {
            GameEventService.Fire(new UnregisterFrog { FrogViewModel = this });
        }

    }


}
