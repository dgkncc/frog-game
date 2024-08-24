using DG.Tweening;
using FrogGame._Core.Common;
using FrogGame.Common.Enums;
using FrogGame.ScriptableObjects;
using System.Collections;
using UnityEngine;

namespace FrogGame.Cell.Contents.GrapeContent
{
    public class Grape : CellContentBase
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private GameObject models;

        private MaterialPropertyBlock _materialPropertyBlock;

        public override void OnInitialized()
        {
            base.OnInitialized();
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

        public override void InitializeContent(CellContentColor contentColor, CellContentDirection contentDirection, int grapeCount, Vector2Int positionInGrid)
        {
            _cellContentColor = contentColor;
            SetGrapeMaterial();
        }

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

        public void FlashColor(float duration)
        {
            StartCoroutine(FlashColorCoroutine(duration));
        }

        private IEnumerator FlashColorCoroutine(float duration)
        {
            var propBlock = _materialPropertyBlock;
            var propertyID = GlobalConstants.GRAPEMATERIAL_FLASH_COLOR_PROPERTY_ID;
            float halfDuration = duration / 2f;

            float elapsedTime = 0f;
            while (elapsedTime < halfDuration)
            {
                elapsedTime += Time.deltaTime;
                float flashAmount = Mathf.Lerp(0f, 1f, elapsedTime / halfDuration);
                UpdateMaterialProperty(propertyID, flashAmount);
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < halfDuration)
            {
                elapsedTime += Time.deltaTime;
                float flashAmount = Mathf.Lerp(1f, 0f, elapsedTime / halfDuration);
                UpdateMaterialProperty(propertyID, flashAmount);
                yield return null;
            }

            UpdateMaterialProperty(propertyID, 0f);
        }

        private void UpdateMaterialProperty(string propertyID, float value)
        {
            meshRenderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetFloat(propertyID, value);
            meshRenderer.SetPropertyBlock(_materialPropertyBlock);
        }

        public void PunchScale()
        {
            models.transform.DOKill();
            models.transform.DOPunchScale(Vector3.one, GlobalConstants.TWEEN_DURATION_IDEAL, 5, 1);
        }

        public void CompleteGrape(float duration)
        {
            models.transform.DOKill();
            models.transform.DOScale(GlobalConstants.CONTENT_SCALED_DOWN_VECTOR, duration).SetEase(Ease.OutQuad).OnComplete(ReturnToPool);
        }

        private void SetGrapeMaterial()
        {
            var material = GameSettings.Instance.GetGrapeMaterial(_cellContentColor);
            meshRenderer.material = material;
        }


    }


}