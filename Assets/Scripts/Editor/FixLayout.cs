using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Điều chỉnh vị trí layout cho màn hình mobile 1080x1920.
/// Board ở giữa màn hình, header ở trên, buttons ngay dưới header.
/// </summary>
public class FixLayout
{
    public static void Execute()
    {
        var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var root in rootObjects)
        {
            // Fix Board position
            var board = root.transform.Find("Canvas/Board");
            if (board != null)
            {
                var rt = board.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, 150);
                rt.sizeDelta = new Vector2(470, 470);
            }

            // Fix Header position
            var header = root.transform.Find("Canvas/Header");
            if (header != null)
            {
                var rt = header.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, 820);
                rt.sizeDelta = new Vector2(1000, 180);
            }

            // Fix ButtonBar position
            var buttonBar = root.transform.Find("Canvas/ButtonBar");
            if (buttonBar != null)
            {
                var rt = buttonBar.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, 630);
            }

            // Fix WinPanel overlay
            var winPanel = root.transform.Find("Canvas/WinPanel");
            if (winPanel != null)
            {
                winPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
            }

            // Fix GameOverPanel overlay
            var gameOverPanel = root.transform.Find("Canvas/GameOverPanel");
            if (gameOverPanel != null)
            {
                gameOverPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Layout fixed!");
    }
}
