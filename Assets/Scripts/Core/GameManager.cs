using Game2048.UI;
using Game2048.View;
using UnityEngine;

namespace Game2048.Core
{
    /// <summary>
    /// Singleton MonoBehaviour điều phối toàn bộ game loop.
    /// Kết nối Board (model) ↔ BoardView (view) ↔ UIManager ↔ các systems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private BoardView _boardView;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private InputHandler _inputHandler;

        [Header("Settings")]
        [SerializeField] private int _boardSize = 4;

        // Core systems
        private Board _board;
        private ScoreSystem _scoreSystem;
        private UndoSystem _undoSystem;
        private SaveManager _saveManager;

        // Flag tránh nhận input trong khi animation đang chạy
        private bool IsAnimating => _boardView != null && _boardView.IsAnimating;

        // ─── Singleton ───────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitSystems();
        }

        private void InitSystems()
        {
            _scoreSystem = new ScoreSystem();
            _undoSystem = new UndoSystem();
            _saveManager = new SaveManager();
            _board = new Board(_boardSize);
        }

        private void Start()
        {
            // Wire UI buttons
            _uiManager?.WireButtons(NewGame, UndoMove, ContinueAfterWin, NewGame);

            // Subscribe score event
            _scoreSystem.OnScoreChanged += (current, best) =>
            {
                _uiManager?.UpdateScore(current, best);
                _uiManager?.SetUndoButtonInteractable(_undoSystem.HasSnapshot);
            };

            // Subscribe Board events → BoardView
            _board.OnTileSpawned += _boardView.OnTileSpawned;
            _board.OnTileMoved   += _boardView.OnTileMoved;
            _board.OnTilesMerged += _boardView.OnTilesMerged;

            // Subscribe input
            if (_inputHandler != null)
                _inputHandler.OnMoveInput += HandleMoveInput;

            // Init BoardView
            _boardView?.Initialize(_boardSize);

            // Thử load save, nếu không có thì new game
            TryLoadGame();

            // Khởi tạo UI score
            _uiManager?.UpdateScore(_scoreSystem.CurrentScore, _scoreSystem.BestScore);
        }

        private void OnDestroy()
        {
            if (_inputHandler != null)
                _inputHandler.OnMoveInput -= HandleMoveInput;
        }

        // ─── Load / New Game ─────────────────────────────────────────────────────

        private void TryLoadGame()
        {
            var savedState = _saveManager.Load();
            if (savedState != null)
            {
                _boardView?.ClearAll();
                _board.LoadFromState(savedState);
                _scoreSystem.SetScore(savedState.Score);
            }
            else
            {
                StartNewGame();
            }
        }

        private void StartNewGame()
        {
            _boardView?.ClearAll();
            _board.Initialize();
            _scoreSystem.Reset();
            _undoSystem.Clear();
            _uiManager?.SetUndoButtonInteractable(false);
        }

        public void NewGame()
        {
            _uiManager?.HideWinPanel();
            _uiManager?.HideGameOverPanel();
            _saveManager.DeleteSave();
            StartNewGame();
        }

        // ─── Input & Move ────────────────────────────────────────────────────────

        private void HandleMoveInput(Direction direction)
        {
            if (IsAnimating) return;
            ExecuteMove(direction);
        }

        private void ExecuteMove(Direction direction)
        {
            // Lưu snapshot trước khi move
            var snapshot = _board.GetBoardState(_scoreSystem.CurrentScore);
            _undoSystem.SaveSnapshot(snapshot);

            bool moved = _board.Move(direction);

            if (!moved)
            {
                // Nước đi không hợp lệ — xóa snapshot vừa lưu
                _undoSystem.Clear();
                return;
            }

            // Cộng điểm
            _scoreSystem.AddScore(_board.LastMergeScore);

            // Sinh ô mới sau mỗi lần di chuyển thành công
            _board.SpawnRandomTile();

            // Update undo button
            _uiManager?.SetUndoButtonInteractable(_undoSystem.HasSnapshot);

            // Lưu game
            _saveManager.Save(_board.GetBoardState(_scoreSystem.CurrentScore));

            // Kiểm tra thắng/thua (sau animation là lý tưởng, nhưng check ngay vẫn ổn)
            if (_board.IsWon)
            {
                _uiManager?.ShowWinPanel();
                return;
            }

            if (_board.IsGameOver)
            {
                _uiManager?.ShowGameOverPanel(_scoreSystem.CurrentScore);
            }
        }

        // ─── Undo ────────────────────────────────────────────────────────────────

        public void UndoMove()
        {
            if (IsAnimating) return;

            var snapshot = _undoSystem.Undo();
            if (snapshot == null) return;

            _uiManager?.HideGameOverPanel();
            _uiManager?.HideWinPanel();

            _boardView?.ClearAll();
            _board.LoadFromState(snapshot);
            _scoreSystem.SetScore(snapshot.Score);

            _uiManager?.SetUndoButtonInteractable(false);
            _saveManager.Save(snapshot);
        }

        // ─── Continue After Win ──────────────────────────────────────────────────

        public void ContinueAfterWin()
        {
            _board.AcknowledgeWin();
            _uiManager?.HideWinPanel();
        }
    }
}
