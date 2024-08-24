using System.Collections.Generic;
using UnityEngine;

namespace FrogGame.ScriptableObjects.Level
{
    [CreateAssetMenu(fileName = "_LevelsCollectionData", menuName = "ScriptableObjects/LevelsCollectionData", order = 1)]
    public class LevelsCollectionData : ScriptableObject
    {
        public List<LevelData> levels;

    }
}
