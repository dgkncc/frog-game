using FrogGame._Core.Common.CoreEnums;
using FrogGame._Core.EventService;
using FrogGame.Cell;
using FrogGame.Cell.Contents.FrogContent;
using System.Collections.Generic;

namespace FrogGame.Common.Events
{

    #region Struct Events
    public struct InitializeGameBoardEvent
    {
        public int GridWidth;
        public int GridHeight;
        public List<List<CellBlock>> CellBlockColumns;
    }

    public struct LoadCurrentLevel { }

    public struct StartLevel
    {
        public int MoveCount;
    }

    public struct AdjustCamera
    {
        public int GridWidth;
    }

    public struct RegisterFrog
    {
        public FrogViewModel FrogViewModel;
    }

    public struct UnregisterFrog
    {
        public FrogViewModel FrogViewModel;
    }

    public struct CheckLevelFail { }

    public struct EnablePlayerInput { }

    public struct DisablePlayerInput { }

    #endregion

    #region Class Events

    public class FinishLevel : GameEvent
    {
        public GameState FinishGameState;

    }

    public class UpdateUIGamePanel : GameEvent
    {
        public int MoveCount;
    }

    #endregion
}
