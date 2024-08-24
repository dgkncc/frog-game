using FrogGame._Core.Common;
using FrogGame._Core.Managers;
using FrogGame.Common.Enums;
using FrogGame.Common.Structs;
using UnityEngine;

namespace FrogGame.Cell
{
    public abstract class CellContentBase : PoolMonoBehaviour, ICellContent
    {
        protected CellContentData _cellContentData;
        protected CellContentColor _cellContentColor;

        public abstract void InitializeContent(CellContentColor contentColor, CellContentDirection contentDirection, int grapeCount, Vector2Int positionInGrid);

        public CellContentData GetCellContentData()
        {
            return _cellContentData;
        }

        public void SetCellContentData(CellContentData contentData)
        {
            _cellContentData = contentData;
        }

        public override void OnInitialized()
        {
            DeactivateContent();
        }

        public override void OnReturnedToPool()
        {
            ResetContent();
            DeactivateContent();
        }

        public abstract void ActivateContent();
        public abstract void DeactivateContent();

        public Transform GetCellContentTransform() => transform;

        public void ReturnContentToPool()
        {
            PoolManager.Instance.ReturnObjectToPool(this);
        }

        protected virtual void ResetContent()
        {
            transform.localScale = Vector3.one;
        }
    }

}