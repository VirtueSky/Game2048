using System;
using UnityEngine;

namespace Game2048.Core
{
    /// <summary>
    /// Quản lý điểm hiện tại và điểm cao nhất (best score).
    /// Best score được lưu bền vững qua PlayerPrefs.
    /// </summary>
    public class ScoreSystem
    {
        private const string BestScoreKey = "BestScore";

        public int CurrentScore { get; private set; }
        public int BestScore { get; private set; }

        public event Action<int, int> OnScoreChanged; // (currentScore, bestScore)

        public ScoreSystem()
        {
            BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        }

        public void AddScore(int points)
        {
            if (points <= 0) return;
            CurrentScore += points;
            if (CurrentScore > BestScore)
            {
                BestScore = CurrentScore;
                PlayerPrefs.SetInt(BestScoreKey, BestScore);
                PlayerPrefs.Save();
            }
            OnScoreChanged?.Invoke(CurrentScore, BestScore);
        }

        public void SetScore(int score)
        {
            CurrentScore = score;
            if (CurrentScore > BestScore)
            {
                BestScore = CurrentScore;
                PlayerPrefs.SetInt(BestScoreKey, BestScore);
                PlayerPrefs.Save();
            }
            OnScoreChanged?.Invoke(CurrentScore, BestScore);
        }

        public void Reset()
        {
            CurrentScore = 0;
            OnScoreChanged?.Invoke(CurrentScore, BestScore);
        }
    }
}
