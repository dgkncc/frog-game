using FrogGame.Common.Enums;
using System;

namespace FrogGame.Common.Structs
{
    [Serializable]
    public struct CellContentData
    {
        public CellContentType type;
        public CellContentColor color;
        public CellContentDirection direction;
        public int grapeCount;
    }
}
