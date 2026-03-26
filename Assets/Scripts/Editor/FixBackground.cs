using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FixBackground
{
    public static void Execute()
    {
        // Set camera background to beige
        var cam = Camera.main;
        if (cam != null)
        {
            cam.backgroundColor = new Color(0.98f, 0.97f, 0.94f);
            EditorUtility.SetDirty(cam);
        }

        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            var canvas = root.GetComponent<Canvas>();
            if (canvas == null) canvas = root.GetComponentInChildren<Canvas>();
            if (canvas == null) continue;

            // Add full-screen background panel as first child (behind everything)
            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(canvas.transform, false);
            bgGO.transform.SetAsFirstSibling();

            var rt = bgGO.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            var img = bgGO.AddComponent<Image>();
            img.color = new Color(0.98f, 0.97f, 0.94f);
            img.raycastTarget = false;

            // Move board up closer to buttons
            var board = canvas.transform.Find("Board");
            if (board != null)
                board.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 250);

            // Move ButtonBar closer to board
            var buttonBar = canvas.transform.Find("ButtonBar");
            if (buttonBar != null)
                buttonBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 650);

            // Fix WinPanel and GameOverPanel overlay position
            var winPanel = canvas.transform.Find("WinPanel");
            if (winPanel != null)
                winPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 250);

            var gameOverPanel = canvas.transform.Find("GameOverPanel");
            if (gameOverPanel != null)
                gameOverPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 250);
        }

        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Background and layout fixed.");
    }
}
