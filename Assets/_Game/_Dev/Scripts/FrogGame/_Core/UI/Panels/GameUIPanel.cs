using FrogGame._Core.EventService;
using FrogGame.Common.Events;
using TMPro;
using UnityEngine;

namespace FrogGame._Core.UI
{
    public class GameUIPanel : UIPanelBase
    {
        [SerializeField] private TMP_Text moveCountText;

        private void Awake()
        {
            GameEventService.On<UpdateUIGamePanel>(OnUpdateUIGamePanel);
        }

        private void OnDestroy()
        {
            GameEventService.Off<UpdateUIGamePanel>(OnUpdateUIGamePanel);

        }

        private void OnUpdateUIGamePanel(UpdateUIGamePanel e)
        {
            UpdateMoveCountText(e.MoveCount);
        }

        private void UpdateMoveCountText(int moveCount)
        {
            moveCountText.text = moveCount.ToString();
        }
    }
}
