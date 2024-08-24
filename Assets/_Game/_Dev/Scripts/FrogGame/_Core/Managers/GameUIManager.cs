using FrogGame._Core.Common.CoreEnums;
using FrogGame._Core.EventService.CoreEvents;
using FrogGame._Core.UI;
using UnityEngine;

namespace FrogGame._Core.Managers
{
    public class GameUIManager : UIManagerBase
    {
        [SerializeField] private MenuUIPanel menuPanel;
        [SerializeField] private GameUIPanel gamePanel;
        [SerializeField] private CompleteUIPanel completePanel;
        [SerializeField] private FailUIPanel failPanel;

        protected override void OnGameStateChanged(GameStateChanged e)
        {
            CloseAllPanels();

            switch (e.GameState)
            {
                case GameState.Menu:
                    menuPanel.Open();
                    break;
                case GameState.Play:
                    gamePanel.Open();
                    break;
                case GameState.Complete:
                    completePanel.Open();
                    break;
                case GameState.Fail:
                    failPanel.Open();
                    break;
            }
        }
    }
}
