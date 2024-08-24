using DG.Tweening;
using FrogGame._Core.Common;
using FrogGame.Common.Enums;
using FrogGame.ScriptableObjects;
using UnityEngine;

namespace FrogGame.Cell.Contents.ArrowContent
{
    public class Arrow : CellContentBase
    {
        [SerializeField] private SpriteRenderer arrowSpriteRenderer;
        [SerializeField] private GameObject models;

        private CellContentDirection _cellContentDirection;

        public override void ActivateContent()
        {
            models.SetActive(true);
            models.transform.DOScale(GlobalConstants.CONTENT_SCALED_UP_VECTOR, GlobalConstants.TWEEN_DURATION_IDEAL);
        }

        public override void DeactivateContent()
        {
            models.SetActive(false);
            models.transform.localScale = GlobalConstants.CONTENT_SCALED_DOWN_VECTOR;
        }

        public override void InitializeContent(CellContentColor contentColor, CellContentDirection contentDirection, int grapeCount, Vector2Int positionInGrid)
        {
            _cellContentColor = contentColor;
            _cellContentDirection = contentDirection;
            SetArrowSpriteRendererColor();
            SetArrowDirection();
        }

        private void SetArrowSpriteRendererColor()
        {
            var color = GameSettings.Instance.GetArrowColor(_cellContentColor);
            arrowSpriteRenderer.color = color;
        }

        private void SetArrowDirection()
        {
            var rotation = GameSettings.Instance.GetDirectionRotation(_cellContentDirection);
            transform.eulerAngles = rotation;
        }
    }

}