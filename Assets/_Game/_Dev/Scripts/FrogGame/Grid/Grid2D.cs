using UnityEngine;

namespace FrogGame.Grid
{
    public class Grid2D
    {
        public GridNode[,] GridNodes => gridNodes;

        private GridNode[,] gridNodes;

        public Grid2D(Vector3 origin, int width, int height, float cellSize)
        {
            origin -= new Vector3(((width * cellSize) / 2) - (cellSize / 2), 0, ((height * cellSize) / 2) - (cellSize / 2));
            gridNodes = new GridNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 cellPosition = origin + new Vector3(x * cellSize, 0, y * cellSize);
                    gridNodes[x, y] = new GridNode(cellPosition, new Vector2Int(x, y));
                }
            }
        }

        public GridNode GetNode(int x, int y)
        {
            if (IsValid(x, y))
                return gridNodes[x, y];

            return null;
        }

        private bool IsValid(int x, int y)
        {
            if (x >= 0 && x < gridNodes.GetLength(0) && y >= 0 && y < gridNodes.GetLength(1))
                return true;

            return false;
        }
    }

}
