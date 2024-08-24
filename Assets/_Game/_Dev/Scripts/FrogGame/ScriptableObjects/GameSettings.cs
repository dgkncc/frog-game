using AYellowpaper.SerializedCollections;
using FrogGame._Core.Common;
using FrogGame.Common.Enums;
using UnityEngine;

namespace FrogGame.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
    public class GameSettings : SingleScriptableObject<GameSettings>
    {
        public float CellSize { get { return cellSize; } }
        public float CellHeight { get { return cellHeight; } }
        public float CameraHorizontalPadding { get { return cameraHorizontalPadding; } }
        public float TongueExtendDuration { get { return tongueExtendDuration; } }

        [Header("Cell Settings")]
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float cellHeight = 1f;

        [Header("Camera Settings")]
        [SerializeField] private float cameraHorizontalPadding = .25f;

        [Header("Frog Settings")]
        [SerializeField] private float tongueExtendDuration = .22f;

        [Header("Object Settings")]
        [SerializeField] private Material defaultCellMaterial;
        [SerializeField] private SerializedDictionary<CellContentColor, Material> cellMaterials;
        [SerializeField] private SerializedDictionary<CellContentColor, Material> frogMaterials;
        [SerializeField] private SerializedDictionary<CellContentColor, Material> grapeMaterials;
        [SerializeField] private SerializedDictionary<CellContentColor, Color> arrowColors;
        [SerializeField] private SerializedDictionary<CellContentDirection, Vector3> directionRotations;

        public Material GetDefaultCellMaterial() => defaultCellMaterial;

        public Material GetCellMaterial(CellContentColor contentColor)
        {
            if (cellMaterials.TryGetValue(contentColor, out var material))
                return material;

            return null;
        }

        public Material GetFrogMaterial(CellContentColor contentColor)
        {
            if (frogMaterials.TryGetValue(contentColor, out var material))
                return material;

            return null;
        }

        public Material GetGrapeMaterial(CellContentColor contentColor)
        {
            if (grapeMaterials.TryGetValue(contentColor, out var material))
                return material;

            return null;
        }

        public Color GetArrowColor(CellContentColor contentColor)
        {
            if (arrowColors.TryGetValue(contentColor, out var color))
                return color;

            return Color.white;
        }

        public Vector3 GetDirectionRotation(CellContentDirection contentDirection)
        {
            if (directionRotations.TryGetValue(contentDirection, out var rotation))
                return rotation;

            return Vector3.zero;
        }
    }
}