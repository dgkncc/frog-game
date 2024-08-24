using DG.Tweening;
using FrogGame._Core.Common;
using FrogGame.Common.Enums;
using FrogGame.Common.Structs;
using FrogGame.Grid;
using FrogGame.ScriptableObjects;
using UnityEngine;

namespace FrogGame.Cell
{
    public class CellBlock : PoolMonoBehaviour
    {
        public bool IsOutOfPool { get { return _isOutOfPool; } }

        [SerializeField] private MeshRenderer meshRenderer;
        private ICellContent _cellContent;
        private GridNode _assignedNode;
        private bool _isOutOfPool;

        public void InsertCellContent(ICellContent cellContent)
        {
            _cellContent = cellContent;

            var t = ((MonoBehaviour)cellContent).transform;
            t.localPosition = Vector3.zero;

            if (TryGetCellContentData(out CellContentData data))
            {
                var cellContentData = data;
                SetCellMaterial(cellContentData.color);
            }
        }

        public void AssignGridNode(GridNode gridTile) => _assignedNode = gridTile;

        public void ActivateContent()
        {
            _cellContent.ActivateContent();
        }

        public void CompleteCellBlock()
        {
            transform.DOKill();
            transform.DOScale(GlobalConstants.CONTENT_SCALED_DOWN_VECTOR, GlobalConstants.TWEEN_DURATION_IDEAL).SetEase(Ease.InQuad).OnComplete(ReturnCellBlockToPool);
        }

        public void ReturnCellBlockToPool()
        {
            if (_cellContent != null)
                _cellContent.ReturnContentToPool();
            transform.localScale = Vector3.one;
            ReturnToPool();
        }

        private void SetCellMaterial(CellContentColor color)
        {
            var material = GameSettings.Instance.GetCellMaterial(color);
            meshRenderer.material = material;
        }



        public bool TryGetCellContent(out ICellContent cellContent)
        {
            if (_cellContent == null)
            {
                cellContent = null;
                return false;
            }

            cellContent = _cellContent;
            return true;
        }

        public bool TryGetCellContentData(out CellContentData data)
        {
            if (_cellContent == null)
            {
                data = new CellContentData();
                return false;
            }

            data = _cellContent.GetCellContentData();
            return true;
        }

        public void RemoveContent() => _cellContent = null;

        public override void OnReturnedToPool()
        {
            _isOutOfPool = false;
            meshRenderer.material = GameSettings.Instance.GetDefaultCellMaterial();
            _assignedNode = null;
        }

        public override void OnGotFromPool()
        {
            _isOutOfPool = true;
        }
    }
}