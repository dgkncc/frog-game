using FrogGame.Common.Enums;
using FrogGame.Common.Structs;
using UnityEngine;

namespace FrogGame.Cell
{
    public interface ICellContent
    {
        public void SetCellContentData(CellContentData contentData);

        public CellContentData GetCellContentData();

        public void ActivateContent();

        public void DeactivateContent();

        public void InitializeContent(CellContentColor contentColor, CellContentDirection contentDirection, int grapeCount, Vector2Int positionInGrid);

        public Transform GetCellContentTransform();

        public void ReturnContentToPool();
    }
}