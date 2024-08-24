using FrogGame._Core.EventService;
using FrogGame.Common.Events;

namespace FrogGame._Core.UI
{
    public class MenuUIPanel : UIPanelBase
    {
        public void OnPlayButtonPressed()
        {
            Close();
            GameEventService.Fire(new LoadCurrentLevel());
        }
    }
}
