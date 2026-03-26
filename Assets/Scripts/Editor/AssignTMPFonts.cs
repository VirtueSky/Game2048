using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssignTMPFonts
{
    public static void Execute()
    {
        // Load the default TMP font asset installed with TMP Essentials
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");

        if (font == null)
        {
            // Try alternative path
            string[] guids = AssetDatabase.FindAssets("LiberationSans SDF t:TMP_FontAsset");
            if (guids.Length > 0)
                font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                    AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        if (font == null)
        {
            Debug.LogError("LiberationSans SDF font not found. Please import TMP Essential Resources.");
            return;
        }

        int count = 0;
        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            foreach (var tmp in root.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (tmp.font == null)
                {
                    tmp.font = font;
                    EditorUtility.SetDirty(tmp);
                    count++;
                }
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log($"Assigned TMP font to {count} components.");
    }
}
