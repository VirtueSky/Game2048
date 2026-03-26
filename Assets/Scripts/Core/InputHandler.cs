using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Game2048.Core
{
    /// <summary>
    /// Nhận input từ bàn phím (Arrow/WASD) và swipe cảm ứng.
    /// Phát event OnMoveInput với hướng di chuyển tương ứng.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("Swipe Settings")]
        [SerializeField] private float _swipeThreshold = 50f;

        public event Action<Direction> OnMoveInput;

        private InputAction _moveLeftAction;
        private InputAction _moveRightAction;
        private InputAction _moveUpAction;
        private InputAction _moveDownAction;

        private Vector2 _touchStartPos;
        private bool _isTouching;

        private void Awake()
        {
            _moveLeftAction  = new InputAction("MoveLeft",  binding: "<Keyboard>/leftArrow");
            _moveRightAction = new InputAction("MoveRight", binding: "<Keyboard>/rightArrow");
            _moveUpAction    = new InputAction("MoveUp",    binding: "<Keyboard>/upArrow");
            _moveDownAction  = new InputAction("MoveDown",  binding: "<Keyboard>/downArrow");

            _moveLeftAction.AddBinding("<Keyboard>/a");
            _moveRightAction.AddBinding("<Keyboard>/d");
            _moveUpAction.AddBinding("<Keyboard>/w");
            _moveDownAction.AddBinding("<Keyboard>/s");

            _moveLeftAction.performed  += _ => OnMoveInput?.Invoke(Direction.Left);
            _moveRightAction.performed += _ => OnMoveInput?.Invoke(Direction.Right);
            _moveUpAction.performed    += _ => OnMoveInput?.Invoke(Direction.Up);
            _moveDownAction.performed  += _ => OnMoveInput?.Invoke(Direction.Down);
        }

        private void OnEnable()
        {
            _moveLeftAction.Enable();
            _moveRightAction.Enable();
            _moveUpAction.Enable();
            _moveDownAction.Enable();
        }

        private void OnDisable()
        {
            _moveLeftAction.Disable();
            _moveRightAction.Disable();
            _moveUpAction.Disable();
            _moveDownAction.Disable();
        }

        private void OnDestroy()
        {
            _moveLeftAction.Dispose();
            _moveRightAction.Dispose();
            _moveUpAction.Dispose();
            _moveDownAction.Dispose();
        }

        // ─── Swipe (Touch) ───────────────────────────────────────────────────────

        private void Update()
        {
            if (Touchscreen.current == null) return;

            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                _touchStartPos = touch.position.ReadValue();
                _isTouching = true;
            }
            else if (touch.press.wasReleasedThisFrame && _isTouching)
            {
                _isTouching = false;
                Vector2 delta = touch.position.ReadValue() - _touchStartPos;
                if (delta.magnitude >= _swipeThreshold)
                    OnMoveInput?.Invoke(GetSwipeDirection(delta));
            }
        }

        private Direction GetSwipeDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                return delta.x > 0 ? Direction.Right : Direction.Left;
            return delta.y > 0 ? Direction.Up : Direction.Down;
        }

        /// <summary>Dùng trong editor test để giả lập input.</summary>
        public void SimulateMoveForTest(Direction direction) => OnMoveInput?.Invoke(direction);
    }
}