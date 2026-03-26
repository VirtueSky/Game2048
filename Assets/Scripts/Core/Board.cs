using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game2048.Core
{
    public enum Direction { Left, Right, Up, Down }

    /// <summary>
    /// Pure C# model của board game 2048.
    /// Chứa toàn bộ logic: di chuyển, gộp ô, kiểm tra win/lose.
    /// Không kế thừa MonoBehaviour — có thể unit test độc lập.
    /// </summary>
    public class Board
    {
        public int Size { get; private set; }

        // Lưới tile: null = ô trống
        private Tile[,] _grid;

        // Điểm gộp trong nước đi gần nhất (để cộng vào ScoreSystem)
        public int LastMergeScore { get; private set; }

        // Sau khi win, vẫn có thể tiếp tục (không check win nữa)
        private bool _winAcknowledged = false;

        public event Action<Tile> OnTileSpawned;
        public event Action<Tile, int, int> OnTileMoved;   // tile, fromRow, fromCol
        public event Action<Tile, Tile, int, int> OnTilesMerged;     // survivor, absorbed, absorbedFromRow, absorbedFromCol

        public Board(int size = 4)
        {
            Size = size;
            _grid = new Tile[size, size];
        }

        // ─── Khởi tạo ────────────────────────────────────────────────────────────

        public void Initialize()
        {
            Clear();
            SpawnRandomTile();
            SpawnRandomTile();
        }

        public void Clear()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    _grid[r, c] = null;
            LastMergeScore = 0;
            _winAcknowledged = false;
        }

        // ─── Truy cập grid ───────────────────────────────────────────────────────

        public Tile GetTile(int row, int col) => _grid[row, col];

        public bool IsEmpty(int row, int col) => _grid[row, col] == null;

        // ─── Spawn tile mới ──────────────────────────────────────────────────────

        /// <summary>
        /// Đặt tile mới vào ô trống ngẫu nhiên.
        /// 90% giá trị 2, 10% giá trị 4.
        /// </summary>
        public bool SpawnRandomTile()
        {
            var emptyCells = GetEmptyCells();
            if (emptyCells.Count == 0) return false;

            var cell = emptyCells[UnityEngine.Random.Range(0, emptyCells.Count)];
            int value = UnityEngine.Random.value < 0.9f ? 2 : 4;
            var tile = new Tile(cell.x, cell.y, value);
            _grid[cell.x, cell.y] = tile;
            OnTileSpawned?.Invoke(tile);
            return true;
        }

        private List<Vector2Int> GetEmptyCells()
        {
            var list = new List<Vector2Int>();
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (_grid[r, c] == null)
                        list.Add(new Vector2Int(r, c));
            return list;
        }

        // ─── Di chuyển ───────────────────────────────────────────────────────────

        /// <summary>
        /// Thực hiện nước đi theo hướng direction.
        /// Trả về true nếu board thay đổi (nước đi hợp lệ).
        /// </summary>
        public bool Move(Direction direction)
        {
            ResetMergeFlags();
            LastMergeScore = 0;
            bool moved = false;

            // Duyệt các dòng/cột theo thứ tự phù hợp với hướng di chuyển
            for (int primary = 0; primary < Size; primary++)
            {
                // Lấy thứ tự các ô trong dòng/cột này theo hướng di chuyển
                int[] order = GetTraversalOrder(direction, primary);

                for (int i = 0; i < order.Length; i++)
                {
                    int r = direction == Direction.Left || direction == Direction.Right
                        ? primary : order[i];
                    int c = direction == Direction.Left || direction == Direction.Right
                        ? order[i] : primary;

                    if (_grid[r, c] == null) continue;

                    var (newR, newC) = FindTarget(r, c, direction);
                    if (newR == r && newC == c) continue;

                    var movingTile = _grid[r, c];
                    var targetTile = _grid[newR, newC];

                    if (targetTile != null && targetTile.Value == movingTile.Value && !targetTile.IsMerged)
                    {
                        // Gộp ô: ô di chuyển biến mất, ô đích nhận giá trị gấp đôi
                        int fromR = movingTile.Row, fromC = movingTile.Col;
                        
                        // Cập nhật giá trị của ô đích trước
                        targetTile.Value *= 2;
                        targetTile.IsMerged = true;
                        LastMergeScore += targetTile.Value;
                        _grid[r, c] = null;
                        
                        // Thông báo merge: absorbed tile di chuyển từ (fromR, fromC) đến vị trí target và biến mất
                        // KHÔNG cập nhật movingTile.Row/Col vì nó sẽ bị xóa
                        OnTilesMerged?.Invoke(targetTile, movingTile, fromR, fromC);
                        moved = true;
                    }
                    else if (targetTile == null)
                    {
                        // Di chuyển vào ô trống
                        int fromR = movingTile.Row, fromC = movingTile.Col;
                        _grid[newR, newC] = movingTile;
                        _grid[r, c] = null;
                        movingTile.Row = newR;
                        movingTile.Col = newC;
                        OnTileMoved?.Invoke(movingTile, fromR, fromC);
                        moved = true;
                    }
                }
            }

            return moved;
        }

        /// <summary>Tìm vị trí đích cuối cùng của tile tại (r,c) theo hướng di chuyển.</summary>
        private (int row, int col) FindTarget(int r, int c, Direction dir)
        {
            int dr = 0, dc = 0;
            switch (dir)
            {
                case Direction.Left:  dc = -1; break;
                case Direction.Right: dc =  1; break;
                case Direction.Up:    dr = -1; break;
                case Direction.Down:  dr =  1; break;
            }

            int targetR = r, targetC = c;
            while (true)
            {
                int nextR = targetR + dr;
                int nextC = targetC + dc;
                if (nextR < 0 || nextR >= Size || nextC < 0 || nextC >= Size) break;

                if (_grid[nextR, nextC] == null)
                {
                    targetR = nextR;
                    targetC = nextC;
                }
                else if (_grid[nextR, nextC].Value == _grid[r, c].Value && !_grid[nextR, nextC].IsMerged)
                {
                    targetR = nextR;
                    targetC = nextC;
                    break;
                }
                else
                {
                    break;
                }
            }
            return (targetR, targetC);
        }

        /// <summary>
        /// Trả về thứ tự index duyệt cho một dòng/cột dựa trên hướng di chuyển.
        /// Ô gần đích nhất được xử lý trước để tránh gộp sai.
        /// </summary>
        private int[] GetTraversalOrder(Direction dir, int primary)
        {
            int[] order = new int[Size];
            if (dir == Direction.Right || dir == Direction.Down)
            {
                for (int i = 0; i < Size; i++) order[i] = Size - 1 - i;
            }
            else
            {
                for (int i = 0; i < Size; i++) order[i] = i;
            }
            return order;
        }

        private void ResetMergeFlags()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    _grid[r, c]?.ResetMergeFlag();
        }

        // ─── Win / Game Over ─────────────────────────────────────────────────────

        public bool IsWon
        {
            get
            {
                if (_winAcknowledged) return false;
                for (int r = 0; r < Size; r++)
                    for (int c = 0; c < Size; c++)
                        if (_grid[r, c] != null && _grid[r, c].Value >= 2048)
                            return true;
                return false;
            }
        }

        /// <summary>Gọi khi người chơi chọn "Continue" sau khi win.</summary>
        public void AcknowledgeWin() => _winAcknowledged = true;

        public bool IsGameOver
        {
            get
            {
                if (GetEmptyCells().Count > 0) return false;
                // Kiểm tra còn cặp liền kề nào có thể gộp không
                for (int r = 0; r < Size; r++)
                    for (int c = 0; c < Size; c++)
                    {
                        int v = _grid[r, c].Value;
                        if (r + 1 < Size && _grid[r + 1, c].Value == v) return false;
                        if (c + 1 < Size && _grid[r, c + 1].Value == v) return false;
                    }
                return true;
            }
        }

        // ─── Snapshot / Restore ──────────────────────────────────────────────────

        public BoardState GetBoardState(int currentScore)
        {
            var state = new BoardState(Size, currentScore);
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    state.SetValue(r, c, _grid[r, c]?.Value ?? 0);
            return state;
        }

        public void LoadFromState(BoardState state)
        {
            Clear();
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                {
                    int v = state.GetValue(r, c);
                    if (v > 0)
                    {
                        var tile = new Tile(r, c, v);
                        _grid[r, c] = tile;
                        OnTileSpawned?.Invoke(tile);
                    }
                }
        }
    }
}