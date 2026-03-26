using System;
using System.Collections;
using System.Collections.Generic;
using Game2048.Core;
using Game2048.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.View
{
    /// <summary>
    /// Quản lý render toàn bộ board: ô nền và TileView.
    /// Sync với Board model sau mỗi nước đi.
    /// </summary>
    public class BoardView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TileView _tilePrefab;
        [SerializeField] private TileColorConfig _colorConfig;
        [SerializeField] private GameObject _cellBackgroundPrefab;

        [Header("Layout")]
        [SerializeField] private float _cellSize = 100f;
        [SerializeField] private float _cellSpacing = 12f;

        private int _size;
        private RectTransform _rectTransform;

        // Map từ (row, col) → TileView đang hiển thị
        private Dictionary<(int, int), TileView> _tileViews = new();

        // Flag: đang có animation chưa xong
        public bool IsAnimating
        {
            get
            {
                foreach (var tv in _tileViews.Values)
                    if (tv != null && tv.IsAnimating) return true;
                return false;
            }
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        // ─── Khởi tạo ────────────────────────────────────────────────────────────

        public void Initialize(int size)
        {
            _size = size;
            float totalSize = size * _cellSize + (size - 1) * _cellSpacing;
            _rectTransform.sizeDelta = new Vector2(totalSize, totalSize);

            // Tạo ô nền
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                {
                    var cell = Instantiate(_cellBackgroundPrefab, transform);
                    var rt = cell.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(_cellSize, _cellSize);
                    rt.anchoredPosition = GetAnchoredPosition(r, c);
                }
        }

        // ─── Refresh sau nước đi ─────────────────────────────────────────────────

        /// <summary>
        /// Đọc trạng thái Board và đồng bộ TileView.
        /// Gọi sau mỗi nước đi hợp lệ.
        /// </summary>
        public void RefreshFromBoard(Board board)
        {
            // Xoá TileView không còn trong board
            var toRemove = new List<(int, int)>();
            foreach (var kv in _tileViews)
            {
                int r = kv.Key.Item1, c = kv.Key.Item2;
                if (board.GetTile(r, c) == null || board.GetTile(r, c).Value == 0)
                    toRemove.Add(kv.Key);
            }
            foreach (var key in toRemove)
            {
                Destroy(_tileViews[key].gameObject);
                _tileViews.Remove(key);
            }

            // Tạo hoặc cập nhật TileView
            for (int r = 0; r < _size; r++)
                for (int c = 0; c < _size; c++)
                {
                    var tile = board.GetTile(r, c);
                    if (tile == null) continue;

                    if (!_tileViews.TryGetValue((r, c), out var tv) || tv == null)
                    {
                        tv = CreateTileView(r, c, tile.Value);
                        tv.PlaySpawnAnimation();
                    }
                    else
                    {
                        tv.SetValue(tile.Value);
                    }
                }
        }

        // ─── Xử lý events từ Board ───────────────────────────────────────────────

        public void OnTileSpawned(Tile tile)
        {
            var tv = CreateTileView(tile.Row, tile.Col, tile.Value);
            tv.PlaySpawnAnimation();
        }

        public void OnTileMoved(Tile tile, int fromRow, int fromCol)
        {
            if (!_tileViews.TryGetValue((fromRow, fromCol), out var tv)) return;
            _tileViews.Remove((fromRow, fromCol));
            _tileViews[(tile.Row, tile.Col)] = tv;
            tv.MoveTo(GetAnchoredPosition(tile.Row, tile.Col));
        }

        public void OnTilesMerged(Tile survivor, Tile absorbed, int absorbedFromRow, int absorbedFromCol)
        {
            // Di chuyển TileView của absorbed đến vị trí survivor
            if (_tileViews.TryGetValue((absorbedFromRow, absorbedFromCol), out var absorbedView))
            {
                absorbedView.MoveTo(GetAnchoredPosition(survivor.Row, survivor.Col));
                _tileViews.Remove((absorbedFromRow, absorbedFromCol));
                // Xóa absorbed view sau khi animation xong (delay nhỏ)
                Destroy(absorbedView.gameObject, 0.15f);
            }

            // Cập nhật giá trị và animation cho survivor
            if (_tileViews.TryGetValue((survivor.Row, survivor.Col), out var survivorView))
            {
                survivorView.SetValue(survivor.Value);
                survivorView.PlayMergeAnimation();
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private TileView CreateTileView(int row, int col, int value)
        {
            var tv = Instantiate(_tilePrefab, transform);
            tv.Init(_colorConfig);
            tv.SetValue(value);
            tv.SetPosition(GetAnchoredPosition(row, col));
            _tileViews[(row, col)] = tv;
            return tv;
        }

        private Vector2 GetAnchoredPosition(int row, int col)
        {
            float totalSize = _size * _cellSize + (_size - 1) * _cellSpacing;
            float x = col * (_cellSize + _cellSpacing) - totalSize / 2f + _cellSize / 2f;
            float y = -row * (_cellSize + _cellSpacing) + totalSize / 2f - _cellSize / 2f;
            return new Vector2(x, y);
        }

        public void ClearAll()
        {
            foreach (var tv in _tileViews.Values)
                if (tv != null) Destroy(tv.gameObject);
            _tileViews.Clear();
        }
    }
}