using FrogGame.Common.Enums;
using UnityEngine;

namespace FrogGame.Cell.Contents.FrogContent
{
    public class FrogModel
    {
        public CellContentColor CellContentColor { get; private set; }
        public CellContentDirection CellContentDirection { get; private set; }
        public int GrapeCount { get; private set; }
        public Vector2Int PositionInGrid { get; private set; }

        public FrogModel(CellContentColor contentColor, CellContentDirection contentDirection, int grapeCount, Vector2Int positionInGrid)
        {
            CellContentColor = contentColor;
            CellContentDirection = contentDirection;
            GrapeCount = grapeCount;
            PositionInGrid = positionInGrid;
        }

        public void UpdateContent(CellContentColor contentColor, CellContentDirection contentDirection, int grapeCount, Vector2Int positionInGrid)
        {
            CellContentColor = contentColor;
            CellContentDirection = contentDirection;
            GrapeCount = grapeCount;
            PositionInGrid = positionInGrid;
        }

    }


}
