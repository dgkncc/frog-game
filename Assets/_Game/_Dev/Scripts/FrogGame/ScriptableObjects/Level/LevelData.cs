using FrogGame.Common.Structs;
using System.Collections.Generic;
using UnityEngine;

namespace FrogGame.ScriptableObjects.Level
{

    [CreateAssetMenu(fileName = "Level_", menuName = "ScriptableObjects/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int moveCount = 10;
        public int width = 4;
        public int height = 4;
        public List<CellContentCollectionData> gridCellContents = new List<CellContentCollectionData>();

    }
}
