using UnityEditor;
using UnityEngine;

public class CheckInputSettings
{
    public static void Execute()
    {
        // Check Active Input Handling (0=Old, 1=New, 2=Both)
        var setting = PlayerSettings.GetPropertyInt("activeInputHandler", BuildTargetGroup.Standalone);
        Debug.Log($"Active Input Handling: {setting} (0=Old, 1=New Input System, 2=Both)");
    }
}
