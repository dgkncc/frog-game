using FrogGame._Core.EventService;
using FrogGame.Cell.Contents.FrogContent;
using FrogGame.Common.Events;
using UnityEngine;

namespace FrogGame.InputService
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _maxDistance = 100f;
        [SerializeField] private LayerMask _frogLayerMask;

        private bool _isEnabled;

        private void OnEnable()
        {
            GameEventService.On<EnablePlayerInput>(OnEnablePlayerInput);
            GameEventService.On<DisablePlayerInput>(OnDisablePlayerInput);
        }

        private void OnDisable()
        {
            GameEventService.Off<EnablePlayerInput>(OnEnablePlayerInput);
            GameEventService.Off<DisablePlayerInput>(OnDisablePlayerInput);
        }

        private void OnEnablePlayerInput(EnablePlayerInput _) => EnableInput();

        private void OnDisablePlayerInput(DisablePlayerInput _) => DisableInput();

        private void EnableInput() => _isEnabled = true;

        private void DisableInput() => _isEnabled = false;

        private void Update()
        {
            if (!_isEnabled)
                return;

            if (Input.GetMouseButtonDown(1))
            {
                HandleMouseClick();
            }
        }

        private void HandleMouseClick()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _frogLayerMask))
            {
                if (hit.collider.TryGetComponent<FrogView>(out var frogView))
                {
                    frogView.ReceiveInput();
                }
            }
        }


    }

}
