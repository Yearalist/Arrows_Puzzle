using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ArrowLevelEditor : EditorWindow
{
    // Grid settings
    private int gridWidth = 6;
    private int gridHeight = 6;
    private int levelNumber = 1;
    private int threeStarMoves = 5;
    private int twoStarMoves = 8;

    // Editor state
    private enum EditorTool { Select, PlaceHead, PlaceBody, Erase }
    private EditorTool currentTool = EditorTool.PlaceHead;
    private ArrowDirection currentDirection = ArrowDirection.Right;
    private int selectedArrowIndex = -1;

    // Arrow data being edited
    private List<ArrowEditorData> arrows = new List<ArrowEditorData>();

    // Visual settings
    private float cellDrawSize = 40f;
    private Vector2 gridOffset = new Vector2(20f, 20f);
    private Vector2 scrollPosition;

    // Colors
    private Color emptyCellColor = new Color(0.85f, 0.85f, 0.9f);
    private Color headColor = new Color(0.15f, 0.15f, 0.2f);
    private Color bodyColor = new Color(0.3f, 0.3f, 0.4f);
    private Color selectedColor = new Color(0.2f, 0.6f, 1f);
    private Color hoverColor = new Color(0.7f, 0.9f, 0.7f);


    [System.Serializable]
    private class ArrowEditorData
    {
        public int headX;
        public int headY;
        public ArrowDirection direction;
        public List<Vector2Int> bodyParts = new List<Vector2Int>();
        public bool isExpanded = true;
    }


    [MenuItem("Tools/Arrow Level Editor")]
    public static void OpenWindow()
    {
        ArrowLevelEditor window = GetWindow<ArrowLevelEditor>("Level Editor");
        window.minSize = new Vector2(800, 600);
        window.Show();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawToolbar();
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        // Left panel - Tools and Arrow List
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        DrawGridSettings();
        EditorGUILayout.Space(10);
        DrawToolSelection();
        EditorGUILayout.Space(10);
        DrawArrowList();
        EditorGUILayout.Space(10);
        DrawSaveLoadButtons();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        // Right panel - Grid Preview
        DrawGridPreview();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("New Level", EditorStyles.toolbarButton))
        {
            if (EditorUtility.DisplayDialog("New Level", "Clear current level?", "Yes", "No"))
            {
                ClearLevel();
            }
        }

        if (GUILayout.Button("Load Level", EditorStyles.toolbarButton))
        {
            LoadLevelFromAsset();
        }

        if (GUILayout.Button("Save Level", EditorStyles.toolbarButton))
        {
            SaveLevelToAsset();
        }

        GUILayout.FlexibleSpace();

        GUILayout.Label($"Arrows: {arrows.Count}", EditorStyles.toolbarButton);

        EditorGUILayout.EndHorizontal();
    }

    private void DrawGridSettings()
    {
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        gridWidth = EditorGUILayout.IntSlider("Width", gridWidth, 3, 12);
        gridHeight = EditorGUILayout.IntSlider("Height", gridHeight, 3, 12);
        if (EditorGUI.EndChangeCheck())
        {
            // Remove arrows outside new grid bounds
            CleanupOutOfBoundsArrows();
        }

        levelNumber = EditorGUILayout.IntField("Level Number", levelNumber);
        threeStarMoves = EditorGUILayout.IntField("3 Star Moves", threeStarMoves);
        twoStarMoves = EditorGUILayout.IntField("2 Star Moves", twoStarMoves);
    }

    private void DrawToolSelection()
    {
        EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = currentTool == EditorTool.Select ? selectedColor : Color.white;
        if (GUILayout.Button("Select", GUILayout.Height(30)))
        {
            currentTool = EditorTool.Select;
        }

        GUI.backgroundColor = currentTool == EditorTool.PlaceHead ? selectedColor : Color.white;
        if (GUILayout.Button("Place Head", GUILayout.Height(30)))
        {
            currentTool = EditorTool.PlaceHead;
        }

        GUI.backgroundColor = currentTool == EditorTool.PlaceBody ? selectedColor : Color.white;
        if (GUILayout.Button("Place Body", GUILayout.Height(30)))
        {
            currentTool = EditorTool.PlaceBody;
        }

        GUI.backgroundColor = currentTool == EditorTool.Erase ? Color.red : Color.white;
        if (GUILayout.Button("Erase", GUILayout.Height(30)))
        {
            currentTool = EditorTool.Erase;
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        // Direction selector (only for PlaceHead)
        if (currentTool == EditorTool.PlaceHead)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Arrow Direction", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = currentDirection == ArrowDirection.Up ? selectedColor : Color.white;
            if (GUILayout.Button("↑ Up", GUILayout.Height(30)))
                currentDirection = ArrowDirection.Up;

            GUI.backgroundColor = currentDirection == ArrowDirection.Down ? selectedColor : Color.white;
            if (GUILayout.Button("↓ Down", GUILayout.Height(30)))
                currentDirection = ArrowDirection.Down;

            GUI.backgroundColor = currentDirection == ArrowDirection.Left ? selectedColor : Color.white;
            if (GUILayout.Button("← Left", GUILayout.Height(30)))
                currentDirection = ArrowDirection.Left;

            GUI.backgroundColor = currentDirection == ArrowDirection.Right ? selectedColor : Color.white;
            if (GUILayout.Button("→ Right", GUILayout.Height(30)))
                currentDirection = ArrowDirection.Right;

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        // Info for PlaceBody
        if (currentTool == EditorTool.PlaceBody && selectedArrowIndex >= 0)
        {
            EditorGUILayout.HelpBox($"Click grid cells to add body parts to Arrow #{selectedArrowIndex + 1}", MessageType.Info);
        }
        else if (currentTool == EditorTool.PlaceBody && selectedArrowIndex < 0)
        {
            EditorGUILayout.HelpBox("Select an arrow first from the list below, then click grid cells to add body parts.", MessageType.Warning);
        }
    }

    private void DrawArrowList()
    {
        EditorGUILayout.LabelField($"Arrows ({arrows.Count})", EditorStyles.boldLabel);

        for (int i = 0; i < arrows.Count; i++)
        {
            ArrowEditorData arrow = arrows[i];

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            // Selection indicator
            GUI.backgroundColor = selectedArrowIndex == i ? selectedColor : Color.white;

            if (GUILayout.Button($"#{i + 1}", GUILayout.Width(30), GUILayout.Height(25)))
            {
                selectedArrowIndex = i;
                currentTool = EditorTool.PlaceBody;
            }

            GUI.backgroundColor = Color.white;

            // Arrow info
            string dirSymbol = GetDirectionSymbol(arrow.direction);
            GUILayout.Label($"{dirSymbol} ({arrow.headX},{arrow.headY})", GUILayout.Width(80));
            GUILayout.Label($"Body: {arrow.bodyParts.Count}", GUILayout.Width(50));

            // Direction change
            if (GUILayout.Button("↻", GUILayout.Width(25), GUILayout.Height(25)))
            {
                arrow.direction = GetNextDirection(arrow.direction);
                Repaint();
            }

            // Delete button
            GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
            if (GUILayout.Button("X", GUILayout.Width(25), GUILayout.Height(25)))
            {
                arrows.RemoveAt(i);
                if (selectedArrowIndex >= arrows.Count)
                    selectedArrowIndex = arrows.Count - 1;
                Repaint();
                break;
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();
        }
    }

    private string GetDirectionSymbol(ArrowDirection dir)
    {
        switch (dir)
        {
            case ArrowDirection.Up: return "↑";
            case ArrowDirection.Down: return "↓";
            case ArrowDirection.Left: return "←";
            case ArrowDirection.Right: return "→";
            default: return "?";
        }
    }

    private ArrowDirection GetNextDirection(ArrowDirection dir)
    {
        switch (dir)
        {
            case ArrowDirection.Up: return ArrowDirection.Right;
            case ArrowDirection.Right: return ArrowDirection.Down;
            case ArrowDirection.Down: return ArrowDirection.Left;
            case ArrowDirection.Left: return ArrowDirection.Up;
            default: return ArrowDirection.Up;
        }
    }
    private void DrawGridPreview()
    {
        Rect gridRect = GUILayoutUtility.GetRect(
            gridWidth * cellDrawSize + gridOffset.x * 2,
            gridHeight * cellDrawSize + gridOffset.y * 2 + 30
        );

        // Title
        GUI.Label(new Rect(gridRect.x, gridRect.y, 200, 20), "Grid Preview", EditorStyles.boldLabel);

        float startX = gridRect.x + gridOffset.x;
        float startY = gridRect.y + gridOffset.y + 20;

        // Draw cells
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                // Flip Y so (0,0) is bottom-left visually
                int drawY = gridHeight - 1 - y;

                Rect cellRect = new Rect(
                    startX + x * cellDrawSize,
                    startY + drawY * cellDrawSize,
                    cellDrawSize - 2,
                    cellDrawSize - 2
                );

                // Determine cell content
                Color cellColor = GetCellColor(x, y);
                string cellLabel = GetCellLabel(x, y);

                // Draw cell
                EditorGUI.DrawRect(cellRect, cellColor);

                // Draw label
                if (!string.IsNullOrEmpty(cellLabel))
                {
                    GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel);
                    labelStyle.alignment = TextAnchor.MiddleCenter;
                    labelStyle.normal.textColor = Color.white;
                    labelStyle.fontStyle = FontStyle.Bold;
                    GUI.Label(cellRect, cellLabel, labelStyle);
                }

                // Draw coordinate in corner
                GUIStyle coordStyle = new GUIStyle(EditorStyles.miniLabel);
                coordStyle.fontSize = 8;
                coordStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUI.Label(new Rect(cellRect.x + 1, cellRect.y + 1, 30, 12), $"{x},{y}", coordStyle);

                // Handle click
                if (Event.current.type == EventType.MouseDown &&
                    Event.current.button == 0 &&
                    cellRect.Contains(Event.current.mousePosition))
                {
                    HandleCellClick(x, y);
                    Event.current.Use();
                    Repaint();
                }
            }
        }
    }

    private Color GetCellColor(int x, int y)
    {
        // Check if this cell has an arrow head
        for (int i = 0; i < arrows.Count; i++)
        {
            if (arrows[i].headX == x && arrows[i].headY == y)
            {
                return selectedArrowIndex == i ? selectedColor : headColor;
            }

            foreach (Vector2Int part in arrows[i].bodyParts)
            {
                if (part.x == x && part.y == y)
                {
                    return selectedArrowIndex == i ? new Color(0.3f, 0.5f, 0.8f) : bodyColor;
                }
            }
        }

        // Empty cell - checkerboard
        bool isLight = (x + y) % 2 == 0;
        return isLight ? emptyCellColor : new Color(0.78f, 0.78f, 0.85f);
    }

    private string GetCellLabel(int x, int y)
    {
        for (int i = 0; i < arrows.Count; i++)
        {
            if (arrows[i].headX == x && arrows[i].headY == y)
            {
                return GetDirectionSymbol(arrows[i].direction);
            }

            foreach (Vector2Int part in arrows[i].bodyParts)
            {
                if (part.x == x && part.y == y)
                {
                    return "■";
                }
            }
        }

        return "";
    }

    private void HandleCellClick(int x, int y)
    {
        switch (currentTool)
        {
            case EditorTool.PlaceHead:
                PlaceArrowHead(x, y);
                break;

            case EditorTool.PlaceBody:
                PlaceArrowBody(x, y);
                break;

            case EditorTool.Erase:
                EraseCell(x, y);
                break;

            case EditorTool.Select:
                SelectArrowAt(x, y);
                break;
        }
    }

    private void PlaceArrowHead(int x, int y)
    {
        // Check if cell is already occupied
        if (IsCellOccupied(x, y))
        {
            Debug.LogWarning("[LevelEditor] Cell already occupied!");
            return;
        }

        ArrowEditorData newArrow = new ArrowEditorData();
        newArrow.headX = x;
        newArrow.headY = y;
        newArrow.direction = currentDirection;
        arrows.Add(newArrow);

        selectedArrowIndex = arrows.Count - 1;
        currentTool = EditorTool.PlaceBody;

        Debug.Log($"[LevelEditor] Placed {currentDirection} arrow head at ({x}, {y})");
    }

    private void PlaceArrowBody(int x, int y)
    {
        if (selectedArrowIndex < 0 || selectedArrowIndex >= arrows.Count)
        {
            Debug.LogWarning("[LevelEditor] No arrow selected!");
            return;
        }

        if (IsCellOccupied(x, y))
        {
            // If clicking on own body part, remove it
            ArrowEditorData arrow = arrows[selectedArrowIndex];
            for (int i = 0; i < arrow.bodyParts.Count; i++)
            {
                if (arrow.bodyParts[i].x == x && arrow.bodyParts[i].y == y)
                {
                    arrow.bodyParts.RemoveAt(i);
                    Debug.Log($"[LevelEditor] Removed body part at ({x}, {y})");
                    return;
                }
            }

            Debug.LogWarning("[LevelEditor] Cell occupied by another arrow!");
            return;
        }

        arrows[selectedArrowIndex].bodyParts.Add(new Vector2Int(x, y));
        Debug.Log($"[LevelEditor] Added body part at ({x}, {y}) to Arrow #{selectedArrowIndex + 1}");
    }

    private void EraseCell(int x, int y)
    {
        for (int i = arrows.Count - 1; i >= 0; i--)
        {
            if (arrows[i].headX == x && arrows[i].headY == y)
            {
                arrows.RemoveAt(i);
                if (selectedArrowIndex >= arrows.Count)
                    selectedArrowIndex = arrows.Count - 1;
                Debug.Log($"[LevelEditor] Erased arrow at ({x}, {y})");
                return;
            }

            for (int j = arrows[i].bodyParts.Count - 1; j >= 0; j--)
            {
                if (arrows[i].bodyParts[j].x == x && arrows[i].bodyParts[j].y == y)
                {
                    arrows[i].bodyParts.RemoveAt(j);
                    Debug.Log($"[LevelEditor] Erased body part at ({x}, {y})");
                    return;
                }
            }
        }
    }

    private void SelectArrowAt(int x, int y)
    {
        for (int i = 0; i < arrows.Count; i++)
        {
            if (arrows[i].headX == x && arrows[i].headY == y)
            {
                selectedArrowIndex = i;
                return;
            }

            foreach (Vector2Int part in arrows[i].bodyParts)
            {
                if (part.x == x && part.y == y)
                {
                    selectedArrowIndex = i;
                    return;
                }
            }
        }

        selectedArrowIndex = -1;
    }

    private bool IsCellOccupied(int x, int y)
    {
        foreach (ArrowEditorData arrow in arrows)
        {
            if (arrow.headX == x && arrow.headY == y) return true;

            foreach (Vector2Int part in arrow.bodyParts)
            {
                if (part.x == x && part.y == y) return true;
            }
        }
        return false;
    }

    private void DrawSaveLoadButtons()
    {
        EditorGUILayout.LabelField("Save / Load", EditorStyles.boldLabel);

        GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
        if (GUILayout.Button("Save as ScriptableObject", GUILayout.Height(35)))
        {
            SaveLevelToAsset();
        }

        GUI.backgroundColor = new Color(0.4f, 0.6f, 1f);
        if (GUILayout.Button("Load from ScriptableObject", GUILayout.Height(35)))
        {
            LoadLevelFromAsset();
        }

        GUI.backgroundColor = Color.white;
    }

    private void SaveLevelToAsset()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Level",
            $"Level_{levelNumber:00}",
            "asset",
            "Save level data",
            "Assets/_Project/ScriptableObjects/Levels"
        );

        if (string.IsNullOrEmpty(path)) return;

        LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
        levelData.gridWidth = gridWidth;
        levelData.gridHeight = gridHeight;
        levelData.levelNumber = levelNumber;
        levelData.threeStarMoves = threeStarMoves;
        levelData.twoStarMoves = twoStarMoves;

        levelData.arrows = new ArrowData[arrows.Count];

        for (int i = 0; i < arrows.Count; i++)
        {
            ArrowData data = new ArrowData();
            data.headX = arrows[i].headX;
            data.headY = arrows[i].headY;
            data.direction = arrows[i].direction;

            data.bodyParts = new ArrowPartData[arrows[i].bodyParts.Count];
            for (int j = 0; j < arrows[i].bodyParts.Count; j++)
            {
                data.bodyParts[j] = new ArrowPartData
                {
                    x = arrows[i].bodyParts[j].x,
                    y = arrows[i].bodyParts[j].y
                };
            }

            levelData.arrows[i] = data;
        }

        AssetDatabase.CreateAsset(levelData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Saved!", $"Level saved to:\n{path}", "OK");
        Debug.Log($"[LevelEditor] Level saved to {path}");
    }

    private void LoadLevelFromAsset()
    {
        string path = EditorUtility.OpenFilePanel(
            "Load Level",
            "Assets/_Project/ScriptableObjects/Levels",
            "asset"
        );

        if (string.IsNullOrEmpty(path)) return;

        // Convert absolute path to relative
        path = "Assets" + path.Substring(Application.dataPath.Length);

        LevelData levelData = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        if (levelData == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not load level data!", "OK");
            return;
        }

        // Load data into editor
        gridWidth = levelData.gridWidth;
        gridHeight = levelData.gridHeight;
        levelNumber = levelData.levelNumber;
        threeStarMoves = levelData.threeStarMoves;
        twoStarMoves = levelData.twoStarMoves;

        arrows.Clear();

        if (levelData.arrows != null)
        {
            foreach (ArrowData data in levelData.arrows)
            {
                ArrowEditorData editorData = new ArrowEditorData();
                editorData.headX = data.headX;
                editorData.headY = data.headY;
                editorData.direction = data.direction;

                if (data.bodyParts != null)
                {
                    foreach (ArrowPartData part in data.bodyParts)
                    {
                        editorData.bodyParts.Add(new Vector2Int(part.x, part.y));
                    }
                }

                arrows.Add(editorData);
            }
        }

        selectedArrowIndex = -1;
        Repaint();

        Debug.Log($"[LevelEditor] Loaded level {levelNumber} with {arrows.Count} arrows");
    }

    private void ClearLevel()
    {
        arrows.Clear();
        selectedArrowIndex = -1;
        levelNumber = 1;
        gridWidth = 6;
        gridHeight = 6;
        threeStarMoves = 5;
        twoStarMoves = 8;
        Repaint();
    }

    private void CleanupOutOfBoundsArrows()
    {
        for (int i = arrows.Count - 1; i >= 0; i--)
        {
            ArrowEditorData arrow = arrows[i];

            if (arrow.headX >= gridWidth || arrow.headY >= gridHeight)
            {
                arrows.RemoveAt(i);
                continue;
            }

            for (int j = arrow.bodyParts.Count - 1; j >= 0; j--)
            {
                if (arrow.bodyParts[j].x >= gridWidth || arrow.bodyParts[j].y >= gridHeight)
                {
                    arrow.bodyParts.RemoveAt(j);
                }
            }
        }

        if (selectedArrowIndex >= arrows.Count)
            selectedArrowIndex = -1;
    }
}

