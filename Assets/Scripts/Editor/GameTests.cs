using Game2048.Core;
using UnityEngine;

/// <summary>
/// Test script cho game 2048. Chạy trong Edit Mode để test logic thuần.
/// Không cần Play Mode.
/// </summary>
public class GameTests
{
    public static void Execute()
    {
        int passed = 0, failed = 0;

        passed += Test("Board khởi tạo 2 tiles", TestBoardInit);
        passed += Test("Move trái — tiles trượt về trái", TestMoveLeft);
        passed += Test("Gộp ô cùng giá trị", TestMerge);
        passed += Test("Không gộp liên tiếp 3 ô giống nhau", TestNoDoublesMerge);
        passed += Test("IsWon khi có tile 2048", TestWinCondition);
        passed += Test("IsGameOver khi board đầy và không gộp được", TestGameOver);
        passed += Test("Nước đi không hợp lệ trả về false", TestInvalidMove);
        passed += Test("Undo restore board và score", TestUndo);
        passed += Test("SaveManager serialize/deserialize", TestSaveLoad);
        passed += Test("ScoreSystem cộng điểm và update BestScore", TestScoreSystem);

        Debug.Log($"\n=== TEST RESULTS: {passed} passed, {failed} failed ===");

        // Local helper
        int Test(string name, System.Func<bool> fn)
        {
            try
            {
                bool ok = fn();
                if (ok)
                    Debug.Log($"  ✓ {name}");
                else
                {
                    Debug.LogError($"  ✗ FAIL: {name}");
                    failed++;
                    return 0;
                }
                return 1;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"  ✗ EXCEPTION in '{name}': {e.Message}");
                failed++;
                return 0;
            }
        }
    }

    // ─── Test Cases ──────────────────────────────────────────────────────────

    static bool TestBoardInit()
    {
        var board = new Board(4);
        board.Initialize();
        int count = 0;
        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
                if (board.GetTile(r, c) != null) count++;
        return count == 2;
    }

    static bool TestMoveLeft()
    {
        // Board: [0, 0, 0, 4] → after MoveLeft → [4, 0, 0, 0]
        var board = CreateBoardFromValues(new int[,]
        {
            { 0, 0, 0, 4 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        });
        board.Move(Direction.Left);
        return board.GetTile(0, 0)?.Value == 4;
    }

    static bool TestMerge()
    {
        // Board: [2, 2, 0, 0] → after MoveLeft → [4, 0, 0, 0]
        var board = CreateBoardFromValues(new int[,]
        {
            { 2, 2, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        });
        board.Move(Direction.Left);
        return board.GetTile(0, 0)?.Value == 4 && board.GetTile(0, 1) == null;
    }

    static bool TestNoDoublesMerge()
    {
        // Board: [2, 2, 2, 0] → after MoveLeft → [4, 2, 0, 0] (chỉ cặp đầu gộp)
        var board = CreateBoardFromValues(new int[,]
        {
            { 2, 2, 2, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        });
        board.Move(Direction.Left);
        return board.GetTile(0, 0)?.Value == 4 && board.GetTile(0, 1)?.Value == 2;
    }

    static bool TestWinCondition()
    {
        var board = CreateBoardFromValues(new int[,]
        {
            { 1024, 1024, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        });
        board.Move(Direction.Left);
        return board.IsWon;
    }

    static bool TestGameOver()
    {
        // Board đầy, không có 2 ô liền kề cùng giá trị
        var board = CreateBoardFromValues(new int[,]
        {
            {  2,  4,  8, 16 },
            { 32, 64,128,256 },
            {512,  2,  4,  8 },
            { 16, 32, 64,128 }
        });
        return board.IsGameOver;
    }

    static bool TestInvalidMove()
    {
        // Board: [2, 0, 0, 0] → MoveLeft — tiles đã ở bên trái, không thay đổi
        var board = CreateBoardFromValues(new int[,]
        {
            { 2, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        });
        bool moved = board.Move(Direction.Left);
        return !moved; // phải là false
    }

    static bool TestUndo()
    {
        var undo = new UndoSystem();
        var state1 = new BoardState(4, 100);
        state1.SetValue(0, 0, 4);

        undo.SaveSnapshot(state1);
        if (!undo.HasSnapshot) return false;

        var restored = undo.Undo();
        if (restored == null) return false;
        if (undo.HasSnapshot) return false; // snapshot đã xóa sau undo

        return restored.Score == 100 && restored.GetValue(0, 0) == 4;
    }

    static bool TestSaveLoad()
    {
        var save = new SaveManager();
        var state = new BoardState(4, 256);
        state.SetValue(1, 2, 8);
        state.SetValue(3, 3, 16);

        save.Save(state);

        var loaded = save.Load();
        if (loaded == null) return false;

        bool ok = loaded.Score == 256
               && loaded.GetValue(1, 2) == 8
               && loaded.GetValue(3, 3) == 16;

        save.DeleteSave();
        return ok;
    }

    static bool TestScoreSystem()
    {
        var score = new ScoreSystem();
        score.Reset();
        score.AddScore(100);
        score.AddScore(200);
        bool ok = score.CurrentScore == 300 && score.BestScore >= 300;
        PlayerPrefs.DeleteKey("BestScore"); // cleanup
        return ok;
    }

    // ─── Helper ──────────────────────────────────────────────────────────────

    /// <summary>Tạo board từ mảng 2D giá trị, bỏ qua events (để test thuần logic).</summary>
    static Board CreateBoardFromValues(int[,] values)
    {
        int size = values.GetLength(0);
        var state = new BoardState(size, 0);
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                state.SetValue(r, c, values[r, c]);

        var board = new Board(size);
        board.LoadFromState(state);
        return board;
    }
}
