using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "SOs/Input Reader")]
    // Class responsible for reading the player input, and firing off input events
    public class InputReader : ScriptableObject
    {
        public enum SwipeDirection
        {
            Nothing,
            Horizontal,
            Vertical
        }
        public enum InputMode
        {
            Menu,
            Gameplay
        }

        // Gameplay events
        public event UnityAction turnLeftEvent = delegate { };
        public event UnityAction turnLeftCanceledEvent = delegate { };
        public event UnityAction turnRightEvent = delegate { };
        public event UnityAction turnRightCanceledEvent = delegate { };
        public event UnityAction<bool> toggleCarEngineEvent = delegate { };

        public event UnityAction startGameEvent = delegate { };

        // Input enums
        private InputMode mode;
        private SwipeDirection direction;

        // Change modes
        public void EnableMenuMode() => mode = InputMode.Menu;
        public void EnableGameplayMode() => mode = InputMode.Gameplay;

        private void OnEnable()
        {
            EnableMenuMode();
            EnhancedTouchSupport.Enable();
            direction = SwipeDirection.Nothing;
        }
        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        // Called from outside of input reader
        public void TriggerStartGame()
        {
            if (mode == InputMode.Menu) startGameEvent.Invoke();
        }
        #region Car events
        public void ToggleCarEngine(bool on)
        {
            if (mode == InputMode.Gameplay) toggleCarEngineEvent.Invoke(on);
        }
        public void TurnLeft()
        {
            if (mode == InputMode.Gameplay) turnLeftEvent.Invoke();
        }
        public void TurnLeftCancel()
        {
            if (mode == InputMode.Gameplay) turnLeftCanceledEvent.Invoke();
        }
        public void TurnRight()
        {
            if (mode == InputMode.Gameplay) turnRightEvent.Invoke();
        }
        public void TurnRightCancel()
        {
            if (mode == InputMode.Gameplay) turnRightCanceledEvent.Invoke();
        }
        #endregion
        #region Swipe events
        public SwipeDirection GetDirection()
        {
            return direction;
        }
        public void RegisterSwipe(Vector2 dragVector)
        {
            float xValue = Mathf.Abs(dragVector.x);
            float yValue = Mathf.Abs(dragVector.y);
            if (xValue > yValue)
            {
                direction = SwipeDirection.Horizontal;
                return;
            }
            if (yValue > xValue)
            {
                direction = SwipeDirection.Vertical;
                return;
            }
        }
        public void EndSwipe()
        {
            direction = SwipeDirection.Nothing;
        }
        #endregion

        public Touch GetTouchData()
        {
            Touch activeTouch = new Touch();
            if (Touch.activeFingers.Count == 1)
            {
                activeTouch = Touch.activeFingers[0].currentTouch;
                return activeTouch;
            }

            return activeTouch;
        }
    }
}