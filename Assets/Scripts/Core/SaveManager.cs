using UnityEngine;

namespace Game2048.Core
{
    /// <summary>
    /// Serialize/deserialize BoardState thành JSON và lưu vào PlayerPrefs.
    /// Best score được quản lý bởi ScoreSystem, không xóa ở đây.
    /// </summary>
    public class SaveManager
    {
        private const string SaveKey = "GameSave";

        public void Save(BoardState state)
        {
            string json = JsonUtility.ToJson(state);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        /// <summary>Trả về BoardState đã lưu, hoặc null nếu không có save.</summary>
        public BoardState Load()
        {
            if (!PlayerPrefs.HasKey(SaveKey)) return null;
            string json = PlayerPrefs.GetString(SaveKey);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonUtility.FromJson<BoardState>(json);
        }

        public void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }

        public bool HasSave => PlayerPrefs.HasKey(SaveKey);
    }
}