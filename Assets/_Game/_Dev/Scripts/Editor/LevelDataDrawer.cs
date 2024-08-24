using FrogGame.ScriptableObjects.Level;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataDrawer : Editor
{
    SerializedProperty gridCellContents;
    SerializedProperty moveCount;
    SerializedProperty width;
    SerializedProperty height;

    bool[] foldouts;

    void OnEnable()
    {
        gridCellContents = serializedObject.FindProperty("gridCellContents");
        moveCount = serializedObject.FindProperty("moveCount");
        width = serializedObject.FindProperty("width");
        height = serializedObject.FindProperty("height");
        InitializeFoldouts();
    }

    private void InitializeFoldouts()
    {
        foldouts = new bool[height.intValue];
        for (int i = 0; i < foldouts.Length; i++)
        {
            foldouts[i] = true;
        }
    }

    private void ResizeGrid(int newWidth, int newHeight)
    {
        int totalCells = newWidth * newHeight;

        // Resize gridCellContents array
        while (gridCellContents.arraySize < totalCells)
        {
            gridCellContents.InsertArrayElementAtIndex(gridCellContents.arraySize);
        }
        while (gridCellContents.arraySize > totalCells)
        {
            gridCellContents.DeleteArrayElementAtIndex(gridCellContents.arraySize - 1);
        }

        // Reinitialize foldouts if necessary
        if (foldouts.Length != newHeight)
        {
            InitializeFoldouts();
        }
    }

    private void DrawGridCellContents(int width, int height)
    {
        int index = 0;
        for (int y = 0; y < height; y++)
        {
            foldouts[y] = EditorGUILayout.Foldout(foldouts[y], $"Row {y}");

            if (foldouts[y])
            {
                EditorGUI.indentLevel++;
                for (int x = 0; x < width; x++)
                {
                    SerializedProperty element = gridCellContents.GetArrayElementAtIndex(index);
                    if (element != null)
                    {
                        EditorGUILayout.PropertyField(element, new GUIContent($"({x}, {y})"), true);
                    }
                    index++;
                }
                EditorGUI.indentLevel--;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(moveCount);
        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(height);

        int newWidth = width.intValue;
        int newHeight = height.intValue;

        if (newWidth * newHeight != gridCellContents.arraySize)
        {
            ResizeGrid(newWidth, newHeight);
        }

        EditorGUILayout.LabelField("Grid Cell Contents");
        EditorGUI.indentLevel++;

        DrawGridCellContents(newWidth, newHeight);

        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
