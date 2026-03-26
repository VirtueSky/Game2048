using DG.Tweening;
using Game2048.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.View
{
    /// <summary>
    /// View của một tile: hiển thị giá trị, màu sắc và animation.
    /// Được quản lý bởi BoardView.
    /// </summary>
    public class TileView : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _valueText;

        private TileColorConfig _colorConfig;
        private RectTransform _rectTransform;

        // Callback để BoardView biết animation đã xong
        public bool IsAnimating { get; private set; }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Init(TileColorConfig colorConfig)
        {
            _colorConfig = colorConfig;
        }

        // ─── Giá trị & màu ───────────────────────────────────────────────────────

        public void SetValue(int value)
        {
            _valueText.text = value.ToString();
            var (bg, text) = _colorConfig.GetColors(value);
            _background.color = bg;
            _valueText.color = text;
        }

        // ─── Animations ──────────────────────────────────────────────────────────

        public void PlaySpawnAnimation()
        {
            transform.localScale = Vector3.zero;
            IsAnimating = true;
            transform.DOScale(1f, 0.15f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => IsAnimating = false);
        }

        public void PlayMergeAnimation()
        {
            IsAnimating = true;
            transform.DOScale(1.2f, 0.08f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                    transform.DOScale(1f, 0.06f)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() => IsAnimating = false));
        }

        /// <summary>Di chuyển tile đến vị trí thế giới targetPosition (anchoredPosition).</summary>
        public void MoveTo(Vector2 targetAnchoredPos, System.Action onComplete = null)
        {
            IsAnimating = true;
            _rectTransform.DOAnchorPos(targetAnchoredPos, 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    IsAnimating = false;
                    onComplete?.Invoke();
                });
        }

        public void SetPosition(Vector2 anchoredPos)
        {
            _rectTransform.anchoredPosition = anchoredPos;
        }
    }
}