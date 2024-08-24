using FrogGame.Common.Enums;
using FrogGame.Common.Structs;
using FrogGame.ScriptableObjects.Level;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelDataEditorWindow : EditorWindow
{
    private LevelData levelData;
    private Vector2 scrollPosition;

    private const float squareSize = 50f;
    private const float sizeReduction = 5f;
    private Rect previewRect = new Rect(0, 400, 300, 300);

    [MenuItem("Custom/Level Data Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelDataEditorWindow));
    }

    void OnGUI()
    {
        GUILayout.Label("Level Data Editor", EditorStyles.boldLabel);

        levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);

        if (levelData == null)
        {
            EditorGUILayout.HelpBox("Drag a LevelData ScriptableObject here.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space();

        GUILayout.Label("Grid Configuration", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Move Count:", EditorStyles.boldLabel);
        levelData.moveCount = EditorGUILayout.IntField("Move Count", levelData.moveCount);
        GUILayout.Label("Width:");
        levelData.width = EditorGUILayout.IntField(levelData.width);
        GUILayout.Label("Height:");
        levelData.height = EditorGUILayout.IntField(levelData.height);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.Label("Grid Contents", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int y = levelData.height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < levelData.width; x++)
            {
                GUIContent buttonContent = new GUIContent(x.ToString() + "," + y.ToString());
                if (GUILayout.Button(buttonContent, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    ShowNoedEditor(x, y);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        DrawGridPreview();
    }

    void ShowNoedEditor(int x, int y)
    {
        NodeEditorWindow nodeEditorWindow = EditorWindow.GetWindow<NodeEditorWindow>("Node Editor");
        nodeEditorWindow.Initialize(levelData, x, y);
        nodeEditorWindow.Show();
    }

    void DrawGridPreview()
    {
        if (levelData != null)
        {
            for (int y = levelData.height - 1; y >= 0; y--)
            {
                for (int x = 0; x < levelData.width; x++)
                {
                    int index = y * levelData.width + x;
                    int stackCount = levelData.gridCellContents[index].contentDataCollection.Count;

                    Rect rect = new Rect(previewRect.x + x * squareSize, previewRect.y + (levelData.height - 1 - y) * squareSize, squareSize, squareSize);
                    for (int i = 0; i < stackCount; i++)
                    {
                        float size = squareSize - i * sizeReduction;
                        float offset = i * sizeReduction / 2;
                        Rect squareRect = new Rect(rect.x + offset, rect.y + offset, size, size);

                        CellContentColor color = levelData.gridCellContents[index].contentDataCollection[i].color;
                        Color guiColor = GetColorFromCellContentColor(color);

                        EditorGUI.DrawRect(squareRect, guiColor);

                        GUI.contentColor = Color.black;
                        GUI.Label(squareRect, GetLabelText(levelData.gridCellContents[index].contentDataCollection[i]));
                        GUI.contentColor = Color.white;
                    }
                }
            }
        }
    }

    string GetLabelText(CellContentData cellContent)
    {
        string labelText = "";

        switch (cellContent.type)
        {
            case CellContentType.None:
                labelText += "-";
                break;
            case CellContentType.Frog:
                labelText += "F";
                labelText += ", " + cellContent.grapeCount;
                break;
            case CellContentType.Grape:
                labelText += "G";
                break;
            case CellContentType.Arrow:
                labelText += "A";
                break;
            default:
                labelText += "-";
                break;
        }

        if ((cellContent.type != CellContentType.Frog) && (cellContent.type != CellContentType.Arrow))
            return labelText;

        switch (cellContent.direction)
        {
            case CellContentDirection.Left:
                labelText += "\nLeft";
                break;
            case CellContentDirection.Right:
                labelText += "\nRight";
                break;
            case CellContentDirection.Up:
                labelText += "\nUp";
                break;
            case CellContentDirection.Down:
                labelText += "\nDown";
                break;
            default:
                labelText += "";
                break;
        }

        return labelText;
    }

    Color GetColorFromCellContentColor(CellContentColor color)
    {
        switch (color)
        {
            case CellContentColor.Blue:
                return new Color(0.188f, 0.769f, 0.925f);
            case CellContentColor.Green:
                return new Color(0.502f, 0.957f, 0.110f);
            case CellContentColor.Purple:
                return new Color(0.722f, 0.110f, 0.957f);
            case CellContentColor.Red:
                return new Color(1.000f, 0.173f, 0.047f);
            case CellContentColor.Yellow:
                return new Color(0.973f, 0.957f, 0.110f);
            default:
                return Color.white;
        }
    }
}

public class NodeEditorWindow : EditorWindow
{
    private LevelData levelData;
    private int xIndex;
    private int yIndex;

    private CellContentData newCellContentData;

    public void Initialize(LevelData data, int x, int y)
    {
        levelData = data;
        xIndex = x;
        yIndex = y;
    }

    void OnGUI()
    {
        GUILayout.Label("Editing Node: " + xIndex + ", " + yIndex, EditorStyles.boldLabel);

        EditorGUILayout.Space();

        GUILayout.Label("Select Content", EditorStyles.boldLabel);

        newCellContentData.type = (CellContentType)EditorGUILayout.EnumPopup("Type", newCellContentData.type);
        newCellContentData.color = (CellContentColor)EditorGUILayout.EnumPopup("Color", newCellContentData.color);
        newCellContentData.direction = (CellContentDirection)EditorGUILayout.EnumPopup("Direction", newCellContentData.direction);
        newCellContentData.grapeCount = EditorGUILayout.IntField("Grape Count", newCellContentData.grapeCount);

        EditorGUILayout.Space();

        if (GUILayout.Button("Add"))
        {
            AddCellContentData();
            Close();
        }
    }

    void AddCellContentData()
    {
        if (levelData.gridCellContents.Count == 0)
        {
            for (int i = 0; i < levelData.width * levelData.height; i++)
            {
                levelData.gridCellContents.Add(new CellContentCollectionData
                {
                    contentDataCollection = new List<CellContentData>()
                });
            }
        }

        int index = yIndex * levelData.width + xIndex;
        levelData.gridCellContents[index].contentDataCollection.Add(newCellContentData);
    }
}
