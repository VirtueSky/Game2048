using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game2048.Data
{
    [Serializable]
    public struct TileColorData
    {
        public int Value;
        public Color BackgroundColor;
        public Color TextColor;
    }

    /// <summary>
    /// ScriptableObject lưu bảng màu cho từng giá trị tile.
    /// Tạo asset tại Assets/ScriptableObjects/TileColorConfig.asset.
    /// </summary>
    [CreateAssetMenu(fileName = "TileColorConfig", menuName = "Game2048/TileColorConfig")]
    public class TileColorConfig : ScriptableObject
    {
        [SerializeField] private List<TileColorData> _colorData = new List<TileColorData>();

        [Header("Fallback (giá trị > 2048)")]
        [SerializeField] private Color _fallbackBackground = new Color(0.24f, 0.23f, 0.21f);
        [SerializeField] private Color _fallbackText = Color.white;

        public (Color bg, Color text) GetColors(int value)
        {
            foreach (var data in _colorData)
                if (data.Value == value)
                    return (data.BackgroundColor, data.TextColor);
            return (_fallbackBackground, _fallbackText);
        }
    }
}