using SpaceGame.Network;
using SpaceGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceGame
{
    public class PlaneUIHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private PlaneNet plane;
        public PlaneNet Plane { get => plane; set => plane = value; }

        [Header("Components - UI Fields")]
        [SerializeField] private ValueUIField speed;
        [SerializeField] private ValueUIField altitude;
        [SerializeField] private ValueUIField gForce;
        [SerializeField] private ValueUIField aoa;
        [SerializeField] private Vector3UIField drag;
        [SerializeField] private Vector3UIField velocity;
        [SerializeField] private ThrottleBar throttle;

        // Used to hold the health bar reference
        public HealthBar healthBar;


        [Header("Flaps & Airbreak")]
        [SerializeField] private Text airBreakNotification;
        [SerializeField] private Text flapNotification;

        private void Start()
        {
            if (throttle != null)
                throttle.SetMaxThrottle(1f);
        }

        void FixedUpdate()
        {
            if (plane == null) return;

            if (throttle != null)
                throttle.SetThrottle(plane.Throttle);

            if (velocity != null)
                velocity.OnValueChanged(plane.LocalVelocity);

            if (drag != null)
                drag.OnValueChanged(plane.Drag);

            if (airBreakNotification != null)
                airBreakNotification.enabled = plane.AirBreakDeployed;

            if (flapNotification != null)
                flapNotification.enabled = plane.FlapsDeployed;

            if (speed != null)
                speed.OnValueChanged(plane.Velocity.magnitude.ToString());

            if (aoa != null)
                aoa.OnValueChanged(plane.AngleOfAttack.ToString());

            if (gForce != null)
                gForce.OnValueChanged(plane.LocalGForce.y.ToString());

            if (altitude != null)
                altitude.OnValueChanged(plane.transform.position.y.ToString());
        }
    }
}

