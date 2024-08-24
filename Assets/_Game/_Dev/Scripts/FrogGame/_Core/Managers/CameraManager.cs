using FrogGame._Core.Common;
using FrogGame._Core.EventService;
using FrogGame.Common.Events;
using FrogGame.ScriptableObjects;
using UnityEngine;

namespace FrogGame._Core.Managers
{
    public class CameraManager : SingleMonoBehaviour<CameraManager>
    {
        [SerializeField] private Camera _camera;

        private void OnEnable()
        {
            GameEventService.On<AdjustCamera>(OnAdjustCamera);
        }

        private void OnDisable()
        {
            GameEventService.Off<AdjustCamera>(OnAdjustCamera);
        }

        private void OnAdjustCamera(AdjustCamera e)
        {
            AdjustCameraOrtographicSize(e.GridWidth);
        }

        private void AdjustCameraOrtographicSize(int gridWidth)
        {
            var cellSize = GameSettings.Instance.CellSize;
            var padding = GameSettings.Instance.CameraHorizontalPadding;

            float orthoWidth = (gridWidth * cellSize + padding) / 2f;
            _camera.orthographicSize = orthoWidth / _camera.aspect;
        }
    }
}
