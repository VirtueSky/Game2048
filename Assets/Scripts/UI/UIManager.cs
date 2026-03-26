using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.UI
{
    /// <summary>
    /// Quản lý toàn bộ UI: điểm số, panels, trạng thái nút.
    /// Subscribe các events từ GameManager và ScoreSystem.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _bestScoreText;

        [Header("Buttons")]
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _undoButton;

        [Header("Win Panel")]
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _winNewGameButton;

        [Header("Game Over Panel")]
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _finalScoreText;
        [SerializeField] private Button _tryAgainButton;

        private void Start()
        {
            HideWinPanel();
            HideGameOverPanel();
            SetUndoButtonInteractable(false);
        }

        // ─── Score ───────────────────────────────────────────────────────────────

        public void UpdateScore(int current, int best)
        {
            if (_scoreText != null) _scoreText.text = current.ToString();
            if (_bestScoreText != null) _bestScoreText.text = best.ToString();
        }

        // ─── Win Panel ───────────────────────────────────────────────────────────

        public void ShowWinPanel()
        {
            if (_winPanel != null) _winPanel.SetActive(true);
        }

        public void HideWinPanel()
        {
            if (_winPanel != null) _winPanel.SetActive(false);
        }

        // ─── Game Over Panel ─────────────────────────────────────────────────────

        public void ShowGameOverPanel(int finalScore)
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(true);
            if (_finalScoreText != null) _finalScoreText.text = finalScore.ToString();
        }

        public void HideGameOverPanel()
        {
            if (_gameOverPanel != null) _gameOverPanel.SetActive(false);
        }

        // ─── Undo Button ─────────────────────────────────────────────────────────

        public void SetUndoButtonInteractable(bool interactable)
        {
            if (_undoButton == null) return;
            _undoButton.interactable = interactable;
            var group = _undoButton.GetComponent<CanvasGroup>();
            if (group != null) group.alpha = interactable ? 1f : 0.4f;
        }

        // ─── Button Wiring ───────────────────────────────────────────────────────

        public void WireButtons(
            UnityEngine.Events.UnityAction onNewGame,
            UnityEngine.Events.UnityAction onUndo,
            UnityEngine.Events.UnityAction onContinue,
            UnityEngine.Events.UnityAction onTryAgain)
        {
            _newGameButton?.onClick.AddListener(onNewGame);
            _undoButton?.onClick.AddListener(onUndo);
            _continueButton?.onClick.AddListener(onContinue);
            _winNewGameButton?.onClick.AddListener(onNewGame);
            _tryAgainButton?.onClick.AddListener(onTryAgain);
        }
    }
}
