//
// Copyright (c) Brian Hernandez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
//

using UnityEngine;

namespace MFlight.Demo
{
    /// <summary>
    /// Combination of camera rig and controller for aircraft. Requires a properly set
    /// up rig. I highly recommend either using or referencing the included prefab.
    /// </summary>
    public class MouseFlightController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] [Tooltip("Aircraft the rig follows and references")]
        private Plane _plane = null;
        [SerializeField] [Tooltip("Transform of the object the mouse rotates to generate MouseAim position")]
        private Transform _mouseAim = null;
        [SerializeField] [Tooltip("Transform of the object on the rig which the camera is attached to")]
        private Transform _cameraRig = null;
        [SerializeField] [Tooltip("Transform of the camera itself")]
        private Transform _cam = null;

        [Header("Options")]
        [SerializeField] [Tooltip("Follow aircraft using fixed update loop")]
        private bool _useFixed = true;

        [SerializeField] [Tooltip("How quickly the camera tracks the mouse aim point.")]
        private float _camSmoothSpeed = 5f;

        [SerializeField] [Tooltip("Mouse sensitivity for the mouse flight target")]
        private float _mouseSensitivity = 3f;

        [SerializeField]
        [Tooltip("How quickly the camera swaps between keyboard and mouse camera positions")]
        private float _controlSwapCamSmoothSpeed = 5f;

        [SerializeField] [Tooltip("How far the boresight and mouse flight are from the aircraft")]
        private float _aimDistance = 500f;

        [Space]
        [SerializeField] [Tooltip("How far the boresight and mouse flight are from the aircraft")]
        private bool _showDebugInfo = false;

        private Vector3 _frozenDirection = Vector3.forward;
        private bool _isMouseFrozen = false;
        private bool _hasGoneOffScreen;

        /// <summary>
        /// Flags that returns true if the mouseAim position has moved off screen since the last camera lock has occured.
        /// </summary>
        public bool HasMouseAimGoneOffScreenDuringLock
        {
            get { return _hasGoneOffScreen; }
        }

        /// <summary>
        /// Get a point along the aircraft's boresight projected out to aimDistance meters.
        /// Useful for drawing a crosshair to aim fixed forward guns with, or to indicate what
        /// direction the aircraft is pointed.
        /// </summary>
        public Vector3 BoresightPos
        {
            get
            {
                return _plane == null
                     ? transform.forward * _aimDistance
                     : (_plane.transform.forward * _aimDistance) + _plane.transform.position;
            }
        }

        /// <summary>
        /// Get the position that the mouse is indicating the aircraft should fly, projected
        /// out to aimDistance meters. Also meant to be used to draw a mouse cursor.
        /// </summary>
        public Vector3 MouseAimPos
        {
            get
            {
                if (_mouseAim != null)
                {
                    return IsMouseAimFrozen
                        ? GetFrozenMouseAimPos()
                        : _mouseAim.position + (_mouseAim.forward * _aimDistance);
                }
                else
                {
                    return transform.forward * _aimDistance;
                }
            }
        }

        /// <summary>
        /// Gets a bool indicating if the mouse has been frozen and some mouse controls for plane control should not be
        /// active
        /// </summary>
        public bool IsMouseAimFrozen
        {
            get
            {
                return _isMouseFrozen;
            }
            set
            {
                if (value)
                {
                    _frozenDirection = _mouseAim.forward;
                }
                else
                {
                    if ((CheckAimOffScreen() || HasMouseAimGoneOffScreenDuringLock) && LastManualInputTime > LastCameraLockTime)
                    {
                        _mouseAim.forward = _plane.transform.forward;
                    }
                    else
                    {
                        _mouseAim.forward = _frozenDirection;
                    }
                }

                _isMouseFrozen = value;
            }
        }

        private float _lastCameraLockTime;

        /// <summary>
        /// Time of the last camera lock requested by the user
        /// </summary>
        public float LastCameraLockTime
        {
            get { return _lastCameraLockTime; }
            set
            {
                _lastCameraLockTime = value;
            }
        }

        private float _lastManualInputTime;

        /// <summary>
        /// Time of the manual input (keyboard)detection
        /// </summary>
        public float LastManualInputTime
        {
            get { return _lastManualInputTime; }
            set
            {
                _lastManualInputTime = value;
            }
        }

        private void Awake()
        {
            if (_plane == null)
                Debug.LogError(name + "MouseFlightController - No aircraft transform assigned!");
            if (_mouseAim == null)
                Debug.LogError(name + "MouseFlightController - No mouse aim transform assigned!");
            if (_cameraRig == null)
                Debug.LogError(name + "MouseFlightController - No camera rig transform assigned!");
            if (_cam == null)
                Debug.LogError(name + "MouseFlightController - No camera transform assigned!");

            // To work correctly, the entire rig must not be parented to anything.
            // When parented to something (such as an aircraft) it will inherit those
            // rotations causing unintended rotations as it gets dragged around.
            transform.parent = null;
        }

        private void Update()
        {
            if (_useFixed == false)
                UpdateCameraPos();

            RotateRig();
        }

        /// <summary>
        /// Sets the camera lock state
        /// </summary>
        /// <param name="value"></param>
        public void SetCameraLocked(bool value)
        {
            if (value)
            {
                LastCameraLockTime = Time.time;
                IsMouseAimFrozen = true;
                _hasGoneOffScreen = false;
            }
            else
            {
                IsMouseAimFrozen = false;
            }
        }

        private void FixedUpdate()
        {
            if (_useFixed == true)
                UpdateCameraPos();
        }

        private void RotateRig()
        {
            if (_mouseAim == null || _cam == null || _cameraRig == null)
                return;

            // Mouse input.
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = -Input.GetAxis("Mouse Y") * _mouseSensitivity;

            // Rotate the aim target that the plane is meant to fly towards.
            // Use the camera's axes in world space so that mouse motion is intuitive.
            _mouseAim.Rotate(_cam.right, mouseY, Space.World);
            _mouseAim.Rotate(_cam.up, mouseX, Space.World);

            RotateCameraToMouseAim();
        }

        public bool CheckAimOffScreen()
        {
            //Logic only applies when the mouse aim is frozen.
            if (!IsMouseAimFrozen) return false;

            var viewPos = Camera.main.WorldToViewportPoint(MouseAimPos);
            if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1 && viewPos.z > 0)
            {
                // Object is within the bounds of the viewport so return true.
                return false;
            }

            //Object outside of bound, exit.
            _hasGoneOffScreen = true;
            return true;
        }

        /// <summary>
        /// Adjusts the mouse aim to follow the plane forward vector
        /// </summary>
        /// <param name="forceCamera">Set to true if you want to camera to also move as part of this function</param>
        public void AdjustAim(bool forceCamera = false)
        {
            _mouseAim.transform.rotation = Damp(_mouseAim.transform.rotation, _plane.transform.rotation, _controlSwapCamSmoothSpeed, Time.deltaTime);
            if (forceCamera)
            {
                RotateCameraToMouseAim();
            }
        }

        /// <summary>
        /// Rotates the camera to point towards the mouseAim position. The movement is damped and is framerate independant.
        /// </summary>
        private void RotateCameraToMouseAim()
        {
            // The up vector of the camera normally is aligned to the horizon. However, when
            // looking straight up/down this can feel a bit weird. At those extremes, the camera
            // stops aligning to the horizon and instead aligns to itself.

            Vector3 upVec = (Mathf.Abs(_mouseAim.forward.y) > 0.9f) ? _cameraRig.up : Vector3.up;

            // Smoothly rotate the camera to face the mouse aim.
            _cameraRig.rotation = Damp(_cameraRig.rotation,
                                      Quaternion.LookRotation(_mouseAim.forward, upVec),
                                      _camSmoothSpeed,
                                      Time.deltaTime);
        }

        private Vector3 GetFrozenMouseAimPos()
        {
            if (_mouseAim != null)
                return _mouseAim.position + (_frozenDirection * _aimDistance);
            else
                return transform.forward * _aimDistance;
        }

        private void UpdateCameraPos()
        {
            if (_plane != null)
            {
                // Move the whole rig to follow the aircraft.
                transform.position = _plane.transform.position;
            }
        }

        // Thanks to Rory Driscoll
        // http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
        /// <summary>
        /// Creates dampened motion between a and b that is framerate independent.
        /// </summary>
        /// <param name="a">Initial parameter</param>
        /// <param name="b">Target parameter</param>
        /// <param name="lambda">Smoothing factor</param>
        /// <param name="dt">Time since last damp call</param>
        /// <returns></returns>
        private Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
        {
            return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-lambda * dt));
        }

        private void OnDrawGizmos()
        {
            if (_showDebugInfo == true)
            {
                Color oldColor = Gizmos.color;

                // Draw the boresight position.
                if (_plane != null)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(BoresightPos, 10f);
                }

                if (_mouseAim != null)
                {
                    // Draw the position of the mouse aim position.
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(MouseAimPos, 10f);

                    // Draw axes for the mouse aim transform.
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(_mouseAim.position, _mouseAim.forward * 50f);
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(_mouseAim.position, _mouseAim.up * 50f);
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(_mouseAim.position, _mouseAim.right * 50f);
                }

                Gizmos.color = oldColor;
            }
        }
    }
}
