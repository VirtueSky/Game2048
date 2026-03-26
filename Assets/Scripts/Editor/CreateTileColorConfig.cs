using Game2048.Data;
using UnityEditor;
using UnityEngine;

public class CreateTileColorConfig
{
    public static void Execute()
    {
        var config = ScriptableObject.CreateInstance<TileColorConfig>();

        // Inject default colors via SerializedObject
        var so = new SerializedObject(config);
        var colorDataProp = so.FindProperty("_colorData");

        var tileColors = new (int value, Color bg, Color text)[]
        {
            (2,    new Color(0.93f, 0.89f, 0.85f), new Color(0.47f, 0.43f, 0.40f)),
            (4,    new Color(0.93f, 0.88f, 0.78f), new Color(0.47f, 0.43f, 0.40f)),
            (8,    new Color(0.95f, 0.69f, 0.47f), Color.white),
            (16,   new Color(0.96f, 0.58f, 0.39f), Color.white),
            (32,   new Color(0.96f, 0.49f, 0.37f), Color.white),
            (64,   new Color(0.96f, 0.37f, 0.23f), Color.white),
            (128,  new Color(0.93f, 0.82f, 0.45f), Color.white),
            (256,  new Color(0.93f, 0.80f, 0.38f), Color.white),
            (512,  new Color(0.93f, 0.78f, 0.31f), Color.white),
            (1024, new Color(0.93f, 0.76f, 0.25f), Color.white),
            (2048, new Color(0.93f, 0.74f, 0.18f), Color.white),
        };

        colorDataProp.arraySize = tileColors.Length;
        for (int i = 0; i < tileColors.Length; i++)
        {
            var element = colorDataProp.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("Value").intValue = tileColors[i].value;
            element.FindPropertyRelative("BackgroundColor").colorValue = tileColors[i].bg;
            element.FindPropertyRelative("TextColor").colorValue = tileColors[i].text;
        }
        so.ApplyModifiedProperties();

        AssetDatabase.CreateAsset(config, "Assets/ScriptableObjects/TileColorConfig.asset");
        AssetDatabase.SaveAssets();
        Debug.Log("TileColorConfig created at Assets/ScriptableObjects/TileColorConfig.asset");
    }
}
