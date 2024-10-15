using UnityEngine;
using UnityEngine.InputSystem;

namespace MFlight.Demo
{
    public class PlayerInputControl : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Plane plane;
        [SerializeField] private MouseFlightController controller;

        [Header("Autopilot")]
        [Tooltip("Sensitivity for autopilot flight.")] public float sensitivity = 5f;
        [Tooltip("Angle at which airplane banks fully into target.")] public float aggressiveTurnAngle = 10f;

        [Header("Flight Control Options")]
        [SerializeField] private bool _disableAutoPilotOnCameraLock;

        private bool _throttleUp;
        private bool _throttleDown;
        private Vector3 _overrideInputValues = Vector3.zero;
        private Vector3 _overrideFlags = Vector3.zero;
        private Vector3 _inputSensitivity = Vector3.one;

        //This Vector3 acts as a collection of flags to override the auto pilot features for each axis if keyboard input is detected. 0 is false, 1 is true
        private Vector3 _enableAutoPilot;

        public void SetThrottleInput(InputAction.CallbackContext context)
        {
            if (plane == null) return;

            var input = context.ReadValue<float>();
            if (context.canceled)
            {
                _throttleUp = false;
                _throttleDown = false;
            }

            if (input == 1)
            {
                _throttleUp = true;
                _throttleDown = false;
            }
            else if (input == -1)
            {
                _throttleUp = false;
                _throttleDown = true;
            }
        }

        public void SetYawInput(InputAction.CallbackContext context)
        {
            _overrideInputValues.x = context.ReadValue<float>();
            if (Mathf.Abs(_overrideInputValues.x) > .25)
            {
                _overrideFlags.x = 1;
                _enableAutoPilot.x = 0;
            }

            if (context.performed)
            {
                controller.LastManualInputTime = Time.time;
            }
                

            if (context.canceled)
                _overrideFlags.x = 0;
        }

        public void SetPitchInput(InputAction.CallbackContext context)
        {
            _overrideInputValues.y = context.ReadValue<float>();
            if (Mathf.Abs(_overrideInputValues.y) > .25)
            {
                _overrideFlags.y = 1;
                _enableAutoPilot.y = 0;
            }
            
            if (context.performed)
            {
                controller.LastManualInputTime = Time.time;
            }

            if (context.canceled)
                _overrideFlags.y = 0;
        }

        public void SetRollInput(InputAction.CallbackContext context)
        {
            _overrideInputValues.z = context.ReadValue<float>();
            if (Mathf.Abs(_overrideInputValues.z) > .25)
            {
                _overrideFlags.z = 1;
                _enableAutoPilot.z = 0;
            }

            if (context.performed)
            {
                controller.LastManualInputTime = Time.time;
            }

            if (context.canceled)
                _overrideFlags.z = 0;
        }

        public void SetMouseFreeze(InputAction.CallbackContext context)
        {
            if (controller == null) return;

            // Freeze the mouse aim direction when the free look key is pressed.
            if (context.canceled)
            {
                controller.SetCameraLocked(false);
                _enableAutoPilot = Vector3.one;

            }
            else if (context.performed)
            {
                controller.SetCameraLocked(true);
            }
        }

        public void DeployAirBreak(InputAction.CallbackContext context)
        {
            if (plane == null) return;

            //Toggle the air break;
            plane.AirBreakDeployed = !plane.AirBreakDeployed;
        }

        public void DeployFlaps(InputAction.CallbackContext context)
        {
            if (plane == null) return;

            //Toggle the flaps;
            plane.FlapsDeployed = !plane.FlapsDeployed;
        }

        private void CheckAllInputCancelled()
        {
            if (_overrideFlags == Vector3.zero)
                _enableAutoPilot = Vector3.one;
        }

        private void CheckMouseInputTime()
        {
            var enableAuto = true;
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");

            if (mouseX == 0 && mouseY == 0)
            {
                enableAuto = false;
            }

            if (enableAuto)
            {
                _enableAutoPilot = new Vector3(_overrideFlags.x == 0 ? 1 : 0, _overrideFlags.y == 0 ? 1 : 0, _enableAutoPilot.z); //Vector3.one;
            }
        }

        private void Awake()
        {
            plane = GetComponent<Plane>();

            if (controller == null)
                Debug.LogError(name + ": Plane - Missing reference to MouseFlightController!");
        }

        private void ThrottleControl()
        {
            if (_throttleUp)
                plane.Throttle += 0.01f;
            else if (_throttleDown)
                plane.Throttle -= 0.01f;
        }

        private void PerformPlaneStall()
        {
            plane.Yaw = plane.StallYaw;
            plane.Pitch = plane.StallPitch;
            plane.Roll = plane.StallRoll;

            controller.AdjustAim(true);
            _enableAutoPilot = Vector3.zero;
        }

        private void Update()
        {
            ThrottleControl();
            CheckAllInputCancelled();

            if (plane.IsInStall)
            {
                PerformPlaneStall();
                return;
            }

            CheckMouseInputTime();


            //rollOverride = keyboardRoll != 0;
            //pitchOverride = keyboardPitch != 0;
            //yawOverride = keyboardYaw != 0;

            var autoYaw = 0f;
            var autoPitch = 0f;
            var autoRoll = 0f;

            // When the player commands their own stick input, it should override what the
            // autopilot is trying to do.

            var flag = _disableAutoPilotOnCameraLock && controller.IsMouseAimFrozen;
            var autoFlag = true;
            if (controller.CheckAimOffScreen())
            {
                autoFlag = controller.LastManualInputTime < controller.LastCameraLockTime;
            }
            
            if (controller != null && !flag)
            {
                if (autoFlag)
                    RunAutopilot(controller.MouseAimPos, out autoYaw, out autoPitch, out autoRoll);
            }


            //var auto = (overrideInputValues.x != 0 || overrideInputValues.y != 0);
            //if (auto)
            //    controller.AdjustAim();

            // Use either keyboard or autopilot input.
            plane.Yaw = _enableAutoPilot.x == 0 ? _overrideInputValues.x * _inputSensitivity.x: autoYaw;
            plane.Pitch = _enableAutoPilot.y == 0? _overrideInputValues.y * _inputSensitivity.y : autoPitch;
            plane.Roll = _enableAutoPilot.z == 0 ? _overrideInputValues.z * _inputSensitivity.z : autoRoll;
        }

        private void RunAutopilot(Vector3 flyTarget, out float yaw, out float pitch, out float roll)
        {
            // This is my usual trick of converting the fly to position to local space.
            // You can derive a lot of information from where the target is relative to self.
            var localFlyTarget = transform.InverseTransformPoint(flyTarget).normalized * sensitivity;
            var angleOffTarget = Vector3.Angle(transform.forward, flyTarget - transform.position);

            // IMPORTANT!
            // These inputs are created proportionally. This means it can be prone to
            // overshooting. The physics in this example are tweaked so that it's not a big
            // issue, but in something with different or more realistic physics this might
            // not be the case. Use of a PID controller for each axis is highly recommended.

            // ====================
            // PITCH AND YAW
            // ====================

            // Yaw/Pitch into the target so as to put it directly in front of the aircraft.
            // A target is directly in front the aircraft if the relative X and Y are both
            // zero. Note this does not handle for the case where the target is directly behind.
            yaw = Mathf.Clamp(localFlyTarget.x, -1f, 1f);
            pitch = -Mathf.Clamp(localFlyTarget.y, -1f, 1f);

            // ====================
            // ROLL
            // ====================

            // Roll is a little special because there are two different roll commands depending
            // on the situation. When the target is off axis, then the plane should roll into it.
            // When the target is directly in front, the plane should fly wings level.

            // An "aggressive roll" is input such that the aircraft rolls into the target so
            // that pitching up (handled above) will put the nose onto the target. This is
            // done by rolling such that the X component of the target's position is zeroed.
            var agressiveRoll = Mathf.Clamp(localFlyTarget.x, -1f, 1f);

            // A "wings level roll" is a roll commands the aircraft to fly wings level.
            // This can be done by zeroing out the Y component of the aircraft's right.
            var wingsLevelRoll = transform.right.y;

            // Blend between auto level and banking into the target.
            var wingsLevelInfluence = Mathf.InverseLerp(0f, aggressiveTurnAngle, angleOffTarget);
            roll = Mathf.Lerp(wingsLevelRoll, agressiveRoll, wingsLevelInfluence);
        }
    }
}
