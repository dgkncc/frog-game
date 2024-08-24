using FrogGame._Core.Common;
using FrogGame._Core.EventService;
using FrogGame._Core.EventService.CoreEvents;
using FrogGame._Core.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace FrogGame._Core.UI
{
    public abstract class UIManagerBase : SingleMonoBehaviour<GameUIManager>
    {

        [SerializeField] private List<UIPanelBase> UIPanels;

        protected virtual void OnEnable()
        {
            GameEventService.On<GameStateChanged>(OnGameStateChanged);
        }

        protected virtual void OnDisable()
        {
            GameEventService.Off<GameStateChanged>(OnGameStateChanged);
        }

        protected abstract void OnGameStateChanged(GameStateChanged e);

        protected virtual void CloseAllPanels()
        {
            foreach (var panel in UIPanels)
            {
                panel.Close();
            }
        }

    }

}

