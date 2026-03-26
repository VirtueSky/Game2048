using Game2048.Data;
using Game2048.View;
using Game2048.UI;
using Game2048.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor script để build toàn bộ scene GameScene từ đầu.
/// Tạo: Camera, Canvas, Board, TilePrefab, GameManager, InputHandler.
/// </summary>
public class SetupScene
{
    private static TileColorConfig _colorConfig;

    public static void Execute()
    {
        _colorConfig = AssetDatabase.LoadAssetAtPath<TileColorConfig>(
            "Assets/ScriptableObjects/TileColorConfig.asset");

        if (_colorConfig == null)
        {
            Debug.LogError("TileColorConfig not found! Run CreateTileColorConfig first.");
            return;
        }

        SetupCamera();
        var canvas = CreateMainCanvas();
        var boardViewGO = CreateBoardArea(canvas);
        CreateHeader(canvas);
        CreateButtons(canvas);
        var winPanel = CreateWinPanel(canvas);
        var gameOverPanel = CreateGameOverPanel(canvas);
        var tilePrefab = CreateTilePrefab();
        var cellBgPrefab = CreateCellBackgroundPrefab();
        SetupBoardView(boardViewGO, tilePrefab, cellBgPrefab);
        SetupManagers(canvas, boardViewGO, winPanel, gameOverPanel);

        EditorUtility.SetDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[0]);
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Scene setup complete!");
    }

    // ─── Camera ──────────────────────────────────────────────────────────────

    static void SetupCamera()
    {
        var cam = Camera.main;
        if (cam == null)
        {
            var go = new GameObject("Main Camera");
            go.AddComponent<Camera>();
            go.tag = "MainCamera";
            cam = go.GetComponent<Camera>();
        }
        cam.backgroundColor = new Color(0.74f, 0.68f, 0.63f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.transform.position = new Vector3(0, 0, -10);
    }

    // ─── Canvas ──────────────────────────────────────────────────────────────

    static GameObject CreateMainCanvas()
    {
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        return canvasGO;
    }

    // ─── Header (title + score) ───────────────────────────────────────────────

    static void CreateHeader(GameObject canvas)
    {
        var header = CreatePanel("Header", canvas.transform,
            new Vector2(0, 900), new Vector2(1000, 160));
        header.GetComponent<Image>().color = Color.clear;

        // Title
        var title = CreateTMPText("Title", header.transform, "2048",
            new Vector2(-300, 0), new Vector2(200, 120), 72, FontStyles.Bold);
        title.GetComponent<TextMeshProUGUI>().color = new Color(0.47f, 0.43f, 0.40f);

        // Score panel
        CreateScorePanel("ScorePanel", header.transform, new Vector2(100, 0), "SCORE", "ScoreText");
        // Best panel
        CreateScorePanel("BestPanel", header.transform, new Vector2(320, 0), "BEST", "BestText");
    }

    static GameObject CreateScorePanel(string name, Transform parent, Vector2 pos, string label, string textName)
    {
        var panel = CreatePanel(name, parent, pos, new Vector2(180, 100));
        panel.GetComponent<Image>().color = new Color(0.72f, 0.67f, 0.63f);
        panel.GetComponent<Image>().raycastTarget = false;

        var rt = panel.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);

        CreateTMPText(label + "Label", panel.transform, label,
            new Vector2(0, 22), new Vector2(160, 30), 18, FontStyles.Bold)
            .GetComponent<TextMeshProUGUI>().color = new Color(0.93f, 0.89f, 0.85f);

        var valueGO = CreateTMPText(textName, panel.transform, "0",
            new Vector2(0, -14), new Vector2(160, 44), 28, FontStyles.Bold);
        valueGO.name = textName;
        valueGO.GetComponent<TextMeshProUGUI>().color = Color.white;

        return panel;
    }

    // ─── Buttons ─────────────────────────────────────────────────────────────

    static void CreateButtons(GameObject canvas)
    {
        var buttonBar = CreatePanel("ButtonBar", canvas.transform,
            new Vector2(0, 720), new Vector2(1000, 80));
        buttonBar.GetComponent<Image>().color = Color.clear;

        CreateButton("NewGameButton", buttonBar.transform, new Vector2(-120, 0),
            new Vector2(200, 60), "New Game", new Color(0.72f, 0.67f, 0.63f));
        var undoBtn = CreateButton("UndoButton", buttonBar.transform, new Vector2(120, 0),
            new Vector2(200, 60), "Undo", new Color(0.72f, 0.67f, 0.63f));

        // Add CanvasGroup for alpha control
        undoBtn.AddComponent<CanvasGroup>();
    }

    // ─── Board Area ───────────────────────────────────────────────────────────

    static GameObject CreateBoardArea(GameObject canvas)
    {
        var boardGO = new GameObject("Board");
        boardGO.transform.SetParent(canvas.transform, false);

        var rt = boardGO.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(500, 500);

        var img = boardGO.AddComponent<Image>();
        img.color = new Color(0.72f, 0.67f, 0.63f);
        img.raycastTarget = false;

        return boardGO;
    }

    // ─── Win Panel ────────────────────────────────────────────────────────────

    static GameObject CreateWinPanel(GameObject canvas)
    {
        var panel = CreatePanel("WinPanel", canvas.transform,
            new Vector2(0, 0), new Vector2(700, 400));
        panel.GetComponent<Image>().color = new Color(0.94f, 0.85f, 0.25f, 0.95f);

        CreateTMPText("WinTitle", panel.transform, "You Win!",
            new Vector2(0, 80), new Vector2(600, 100), 60, FontStyles.Bold)
            .GetComponent<TextMeshProUGUI>().color = Color.white;

        CreateButton("ContinueButton", panel.transform, new Vector2(-140, -60),
            new Vector2(220, 60), "Continue", new Color(0.47f, 0.43f, 0.40f));
        CreateButton("WinNewGameButton", panel.transform, new Vector2(140, -60),
            new Vector2(220, 60), "New Game", new Color(0.47f, 0.43f, 0.40f));

        panel.SetActive(false);
        return panel;
    }

    // ─── Game Over Panel ──────────────────────────────────────────────────────

    static GameObject CreateGameOverPanel(GameObject canvas)
    {
        var panel = CreatePanel("GameOverPanel", canvas.transform,
            new Vector2(0, 0), new Vector2(700, 400));
        panel.GetComponent<Image>().color = new Color(0.47f, 0.43f, 0.40f, 0.95f);

        CreateTMPText("GameOverTitle", panel.transform, "Game Over!",
            new Vector2(0, 100), new Vector2(600, 80), 52, FontStyles.Bold)
            .GetComponent<TextMeshProUGUI>().color = Color.white;

        CreateTMPText("GameOverScoreLabel", panel.transform, "Score",
            new Vector2(0, 30), new Vector2(400, 40), 24, FontStyles.Normal)
            .GetComponent<TextMeshProUGUI>().color = new Color(0.8f, 0.8f, 0.8f);

        var finalScore = CreateTMPText("FinalScoreText", panel.transform, "0",
            new Vector2(0, -20), new Vector2(400, 60), 44, FontStyles.Bold);
        finalScore.name = "FinalScoreText";
        finalScore.GetComponent<TextMeshProUGUI>().color = Color.white;

        CreateButton("TryAgainButton", panel.transform, new Vector2(0, -100),
            new Vector2(240, 60), "Try Again", new Color(0.96f, 0.49f, 0.37f));

        panel.SetActive(false);
        return panel;
    }

    // ─── TilePrefab ───────────────────────────────────────────────────────────

    static GameObject CreateTilePrefab()
    {
        var tileGO = new GameObject("TilePrefab");
        tileGO.transform.SetParent(null);

        var rt = tileGO.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 100);

        var img = tileGO.AddComponent<Image>();
        img.color = new Color(0.80f, 0.75f, 0.71f);

        // Text
        var textGO = new GameObject("ValueText");
        textGO.transform.SetParent(tileGO.transform, false);
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = textRT.offsetMax = Vector2.zero;
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "2";
        tmp.fontSize = 40;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(0.47f, 0.43f, 0.40f);

        // TileView component
        var tileView = tileGO.AddComponent<TileView>();
        var so = new SerializedObject(tileView);
        so.FindProperty("_background").objectReferenceValue = img;
        so.FindProperty("_valueText").objectReferenceValue = tmp;
        so.ApplyModifiedProperties();

        // Save as prefab
        var prefab = PrefabUtility.SaveAsPrefabAsset(tileGO, "Assets/Prefabs/TilePrefab.prefab");
        Object.DestroyImmediate(tileGO);
        Debug.Log("TilePrefab created at Assets/Prefabs/TilePrefab.prefab");
        return prefab;
    }

    static GameObject CreateCellBackgroundPrefab()
    {
        var cellGO = new GameObject("CellBackground");
        var rt = cellGO.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 100);
        var img = cellGO.AddComponent<Image>();
        img.color = new Color(0.80f, 0.75f, 0.71f);
        img.raycastTarget = false;

        var prefab = PrefabUtility.SaveAsPrefabAsset(cellGO, "Assets/Prefabs/CellBackground.prefab");
        Object.DestroyImmediate(cellGO);
        return prefab;
    }

    // ─── BoardView setup ─────────────────────────────────────────────────────

    static void SetupBoardView(GameObject boardGO, GameObject tilePrefab, GameObject cellBgPrefab)
    {
        var boardView = boardGO.AddComponent<BoardView>();
        var so = new SerializedObject(boardView);

        var tileView = tilePrefab.GetComponent<TileView>();
        so.FindProperty("_tilePrefab").objectReferenceValue = tileView;
        so.FindProperty("_colorConfig").objectReferenceValue = _colorConfig;
        so.FindProperty("_cellBackgroundPrefab").objectReferenceValue = cellBgPrefab;
        so.FindProperty("_cellSize").floatValue = 108f;
        so.FindProperty("_cellSpacing").floatValue = 14f;
        so.ApplyModifiedProperties();
    }

    // ─── Managers setup ──────────────────────────────────────────────────────

    static void SetupManagers(GameObject canvas, GameObject boardGO,
        GameObject winPanel, GameObject gameOverPanel)
    {
        // InputHandler
        var inputGO = new GameObject("InputHandler");
        var inputHandler = inputGO.AddComponent<InputHandler>();

        // UIManager — assign refs
        var uiManager = canvas.AddComponent<UIManager>();
        AssignUIRefs(uiManager, canvas, winPanel, gameOverPanel);

        // GameManager
        var gmGO = new GameObject("GameManager");
        var gm = gmGO.AddComponent<GameManager>();
        var gmSO = new SerializedObject(gm);
        gmSO.FindProperty("_boardView").objectReferenceValue = boardGO.GetComponent<BoardView>();
        gmSO.FindProperty("_uiManager").objectReferenceValue = uiManager;
        gmSO.FindProperty("_inputHandler").objectReferenceValue = inputHandler;
        gmSO.FindProperty("_boardSize").intValue = 4;
        gmSO.ApplyModifiedProperties();
    }

    static void AssignUIRefs(UIManager uiManager, GameObject canvas,
        GameObject winPanel, GameObject gameOverPanel)
    {
        var so = new SerializedObject(uiManager);

        so.FindProperty("_scoreText").objectReferenceValue =
            FindInHierarchy<TextMeshProUGUI>(canvas, "ScoreText");
        so.FindProperty("_bestScoreText").objectReferenceValue =
            FindInHierarchy<TextMeshProUGUI>(canvas, "BestText");
        so.FindProperty("_newGameButton").objectReferenceValue =
            FindInHierarchy<Button>(canvas, "NewGameButton");
        so.FindProperty("_undoButton").objectReferenceValue =
            FindInHierarchy<Button>(canvas, "UndoButton");
        so.FindProperty("_winPanel").objectReferenceValue = winPanel;
        so.FindProperty("_continueButton").objectReferenceValue =
            FindInHierarchy<Button>(winPanel, "ContinueButton");
        so.FindProperty("_winNewGameButton").objectReferenceValue =
            FindInHierarchy<Button>(winPanel, "WinNewGameButton");
        so.FindProperty("_gameOverPanel").objectReferenceValue = gameOverPanel;
        so.FindProperty("_finalScoreText").objectReferenceValue =
            FindInHierarchy<TextMeshProUGUI>(gameOverPanel, "FinalScoreText");
        so.FindProperty("_tryAgainButton").objectReferenceValue =
            FindInHierarchy<Button>(gameOverPanel, "TryAgainButton");
        so.ApplyModifiedProperties();
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    static GameObject CreatePanel(string name, Transform parent, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        go.AddComponent<Image>();
        return go;
    }

    static GameObject CreateButton(string name, Transform parent, Vector2 pos, Vector2 size,
        string label, Color bgColor)
    {
        var go = CreatePanel(name, parent, pos, size);
        var img = go.GetComponent<Image>();
        img.color = bgColor;

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = bgColor * 1.1f;
        colors.pressedColor = bgColor * 0.9f;
        btn.colors = colors;

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = textRT.offsetMax = Vector2.zero;
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 22;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        return go;
    }

    static GameObject CreateTMPText(string name, Transform parent, string text,
        Vector2 pos, Vector2 size, float fontSize, FontStyles style)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }

    static T FindInHierarchy<T>(GameObject root, string name) where T : Component
    {
        foreach (var t in root.GetComponentsInChildren<T>(true))
            if (t.gameObject.name == name) return t;
        return null;
    }
}
