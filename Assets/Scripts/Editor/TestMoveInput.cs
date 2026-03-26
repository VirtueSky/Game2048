using Game2048.Core;
using UnityEngine;

/// <summary>Test di chuyển bằng cách gọi trực tiếp GameManager từ editor script.</summary>
public class TestMoveInput
{
    public static void Execute()
    {
        var gm = GameObject.FindFirstObjectByType<GameManager>();
        if (gm == null) { Debug.LogError("GameManager not found in scene!"); return; }

        var ih = GameObject.FindFirstObjectByType<InputHandler>();
        if (ih == null) { Debug.LogError("InputHandler not found!"); return; }

        // Check if OnMoveInput has subscribers via reflection
        var field = typeof(InputHandler).GetField("OnMoveInput",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var eventDelegate = field?.GetValue(ih) as System.Delegate;
        int subCount = eventDelegate?.GetInvocationList().Length ?? 0;

        Debug.Log($"GameManager found: {gm.gameObject.name}");
        Debug.Log($"InputHandler found: {ih.gameObject.name}");
        Debug.Log($"OnMoveInput subscribers: {subCount}");

        // Simulate a Left move directly
        ih.SimulateMoveForTest(Direction.Left);
        Debug.Log("Simulated Left move via InputHandler.");
    }
}
