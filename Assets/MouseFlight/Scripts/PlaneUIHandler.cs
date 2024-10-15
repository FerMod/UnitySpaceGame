using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFlight.Demo
{
    public class PlaneUIHandler : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Plane plane;


        [Header("Components - UI Fields")]
        [SerializeField] private ValueUIField speed;
        [SerializeField] private ValueUIField altitude;
        [SerializeField] private ValueUIField gForce;
        [SerializeField] private ValueUIField aoa;
        [SerializeField] private Vector3UIField drag;
        [SerializeField] private Vector3UIField velocity;
        [SerializeField] private Slider throttle;

        [Header("Flaps & Airbreak")]
        [SerializeField] private Text airBreakNotification;
        [SerializeField] private Text flapNotification;

        void FixedUpdate()
        {
            if (plane == null) return;

            if (throttle != null)
                throttle.value = plane.Throttle;

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

