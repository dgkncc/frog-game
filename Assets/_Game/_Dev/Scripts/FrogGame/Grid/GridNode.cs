using DG.Tweening;
using FrogGame._Core.Common;
using FrogGame.Cell;
using System.Collections.Generic;
using UnityEngine;

namespace FrogGame.Grid
{
    public class GridNode
    {
        public Vector3 Position { get { return _position; } }
        public Vector2Int PositionInGrid { get { return _positionInGrid; } }

        public CellBlock ActiveCellBlock { get { return _activeCellBlock; } }

        private Vector3 _position;
        private Vector2Int _positionInGrid;
        private Stack<CellBlock> _cellBlocks;
        private CellBlock _activeCellBlock;

        public GridNode(Vector3 position, Vector2Int positionInGrid)
        {
            _position = position;
            _positionInGrid = positionInGrid;
            _cellBlocks = new Stack<CellBlock>();
        }

        public void InsertCellBlock(CellBlock cellBlock)
        {
            if (cellBlock == null)
                return;
            _cellBlocks.Push(cellBlock);
        }

        public void ActivateNextCellBlock()
        {
            if (_cellBlocks.Count == 0)
                return;

            _activeCellBlock = _cellBlocks.Pop();
            _activeCellBlock.ActivateContent();
        }

        public void CompleteActiveCellBlock(float delay = 0f)
        {
            if (_activeCellBlock == null) return;
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delay);
            sequence.AppendCallback(_activeCellBlock.CompleteCellBlock);
            sequence.AppendInterval(GlobalConstants.TWEEN_DURATION_IDEAL);
            sequence.AppendCallback(ActivateNextCellBlock);
            sequence.Play();
        }

    }
}