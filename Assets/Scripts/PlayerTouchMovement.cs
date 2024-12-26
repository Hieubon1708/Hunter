using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Hunter
{
    public class PlayerTouchMovement : MonoBehaviour
    {
        [SerializeField]
        private Vector2 JoystickSize = new Vector2(300, 300);
        [SerializeField]
        private FloatingJoystick Joystick;
        [SerializeField]
        private NavMeshAgent Player;

        private Finger MovementFinger;
        private Vector2 MovementAmount;
        private Vector2 center;

        public Vector2 GetMovemntAmount()
        {
            return MovementAmount;
        }

        private void Start()
        {
            center = new Vector2(Screen.width / 2, 400);
            Joystick.RectTransform.anchoredPosition = center;
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable(); // starting with Unity 2022 this does not work! You need to attach a TouchSimulation.cs script to your player
            ETouch.Touch.onFingerDown += HandleFingerDown;
            ETouch.Touch.onFingerUp += HandleLoseFinger;
            ETouch.Touch.onFingerMove += HandleFingerMove;
        }

        private void OnDisable()
        {
            ETouch.Touch.onFingerDown -= HandleFingerDown;
            ETouch.Touch.onFingerUp -= HandleLoseFinger;
            ETouch.Touch.onFingerMove -= HandleFingerMove;
            EnhancedTouchSupport.Disable(); // You need to attach a TouchSimulation.cs script to your player
        }

        private void HandleFingerMove(Finger MovedFinger)
        {
            if (MovedFinger == MovementFinger)
            {
                Vector2 knobPosition;
                float maxMovement = JoystickSize.x / 2f;
                ETouch.Touch currentTouch = MovedFinger.currentTouch;
                if (Vector2.Distance(
                        currentTouch.screenPosition,
                        Joystick.RectTransform.anchoredPosition
                    ) > maxMovement)
                {
                    knobPosition = (
                        currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition
                        ).normalized
                        * maxMovement;
                }
                else
                {
                    knobPosition = currentTouch.screenPosition - Joystick.RectTransform.anchoredPosition;
                }
                Joystick.Knob.anchoredPosition = knobPosition;
                MovementAmount = knobPosition / maxMovement;
            }
        }

        private void HandleLoseFinger(Finger LostFinger)
        {
            if (LostFinger == MovementFinger)
            {
                MovementFinger = null;
                Joystick.Knob.anchoredPosition = Vector2.zero;
                MovementAmount = Vector2.zero;
                Joystick.RectTransform.anchoredPosition = center;
            }
        }

        private void HandleFingerDown(Finger TouchedFinger)
        {
            if (MovementFinger == null)
            {
                MovementFinger = TouchedFinger;
                MovementAmount = Vector2.zero;
                Joystick.RectTransform.sizeDelta = JoystickSize;
                Joystick.RectTransform.anchoredPosition = ClampStartPosition(Input.mousePosition);
            }
        }

        private Vector2 ClampStartPosition(Vector2 StartPosition)
        {
            if (StartPosition.x < JoystickSize.x / 2)
            {
                StartPosition.x = JoystickSize.x / 2;
            }
            if (StartPosition.y < JoystickSize.y / 2)
            {
                StartPosition.y = JoystickSize.y / 2;
            }
            else if (StartPosition.x > Screen.width - JoystickSize.x / 2)
            {
                StartPosition.x = Screen.width - JoystickSize.x / 2;
            }
            else if (StartPosition.y > Screen.height - JoystickSize.y / 2)
            {
                StartPosition.y = Screen.height - JoystickSize.y / 2;
            }
            return StartPosition;
        }

        public bool i;

        private void Update()
        {
            if(PlayerController.instance != null && !i)
            {
                Vector3 scaledMovement = Player.speed * Time.deltaTime * new Vector3(
                MovementAmount.x,
                0,
                MovementAmount.y
            );

                Player.transform.LookAt(PlayerController.instance.lookAt.transform.position + scaledMovement, Vector3.up);
                Player.Move(scaledMovement);
            }
        }
    }
}
