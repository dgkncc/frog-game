using FrogGame._Core.Common.CoreEnums;

namespace FrogGame._Core.EventService.CoreEvents
{
    public class GameStateChanged : GameEvent
    {
        public GameState GameState;
    }
}
