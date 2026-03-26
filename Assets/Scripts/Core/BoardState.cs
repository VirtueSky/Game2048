using System;

namespace Game2048.Core
{
    /// <summary>
    /// Snapshot của trạng thái board dùng cho Undo và Save/Load.
    /// Dùng int[] thay vì int[,] để JsonUtility có thể serialize.
    /// </summary>
    [Serializable]
    public class BoardState
    {
        public int Size;
        public int[] Values; // row-major: Values[row * Size + col]
        public int Score;

        public BoardState() { }

        public BoardState(int size, int score)
        {
            Size = size;
            Score = score;
            Values = new int[size * size];
        }

        public int GetValue(int row, int col) => Values[row * Size + col];

        public void SetValue(int row, int col, int value) => Values[row * Size + col] = value;

        /// <summary>Tạo deep copy để tránh reference sharing giữa undo snapshot và board hiện tại.</summary>
        public BoardState Clone()
        {
            var clone = new BoardState(Size, Score);
            Array.Copy(Values, clone.Values, Values.Length);
            return clone;
        }
    }
}